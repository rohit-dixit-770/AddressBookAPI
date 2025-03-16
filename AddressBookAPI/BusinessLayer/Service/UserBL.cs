using System;
using System.Security.Claims;
using System.Web;
using AutoMapper;
using BusinessLayer.Interface;
using Middleware.Email;
using Middleware.Hashing;
using Middleware.JWT;
using Middleware.RabbitMQ;
using ModelLayer.Model;
using Newtonsoft.Json;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly JwtTokenHelper _jwtToken;
        private readonly IMapper _mapper;
        private readonly RabbitMQProducer _rabbitMQProducer;

        public UserBL(IUserRL userRL, JwtTokenHelper jwtToken, IMapper mapper, RabbitMQProducer rabbitMQProducer)
        {
            _userRL = userRL;
            _jwtToken = jwtToken;
            _mapper = mapper;
            _rabbitMQProducer = rabbitMQProducer;
        }

        public bool RegisterUser(RegisterModel request)
        {
            var existingUser = _userRL.GetUser(request.Email);
            if (existingUser != null)
                return false;

            var user = _mapper.Map<UserEntity>(request);
            user.Password = PasswordHelper.HashPassword(request.Password);
            _userRL.AddUser(user);

            // **Publish Event for Email Notification**
            var emailMessage = new EmailMessage
            {
                To = user.Email,
                Subject = "Welcome to HelloApp!",
                Body = "Thank you for registering. Your account has been created successfully."
            };

            string jsonMessage = JsonConvert.SerializeObject(emailMessage);
            _rabbitMQProducer.PublishMessage("emailQueue", jsonMessage); 

            return true;
        }

        public string? LoginUser(LoginModel request)
        {
            var user = _userRL.GetUser(request.Email);
            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.Password))
                return null;

            var userModel = _mapper.Map<UserModel>(user);
            return _jwtToken.GenerateToken(userModel);
        }

        public bool ForgotPassword(string email)
        {
            var user = _userRL.GetUser(email);
            if (user == null) return false;

            string resetToken = _jwtToken.GenerateResetToken(user.Email);
            string encodedToken = HttpUtility.UrlEncode(resetToken);

            var emailMessage = new EmailMessage
            {
                To = user.Email,
                Subject = "Reset Your Password",
                Body = $"Your reset password Token: {encodedToken}"
            };

            string jsonMessage = JsonConvert.SerializeObject(emailMessage);
            _rabbitMQProducer.PublishMessage("emailQueue", jsonMessage);

            return true;
        }

        public bool ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
                return false;

            string decodedToken = HttpUtility.UrlDecode(token);
            var tokenData = _jwtToken.ValidateResetToken(decodedToken);

            if (tokenData == null || !tokenData.ContainsKey(ClaimTypes.Email))
                return false;

            string email = tokenData[ClaimTypes.Email].ToString();
            var user = _userRL.GetUser(email);
            if (user == null)
                return false;

            user.Password = PasswordHelper.HashPassword(newPassword);
            _userRL.UpdateUser(user);

            return true;
        }
    }
}
