using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using BusinessLayer.AutoMapper;
using Middleware.JWT;
using Middleware.Hashing;
using System.Web;
using System.Security.Claims;
using Middleware.Email;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly JwtTokenHelper _jwtToken;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        public UserBL(IUserRL userRL, JwtTokenHelper jwtToken, IMapper mapper, EmailService emailService)
        {
            _userRL = userRL;
            _jwtToken = jwtToken;
            _mapper = mapper;
            _emailService = emailService;
        }
        public string? LoginUser(LoginModel request)
        {
            var user = _userRL.GetUser(request.Email);
            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.Password))
                return null;
            var userModel = _mapper.Map<UserModel>(user);

            return _jwtToken.GenerateToken(userModel);
        }

        public bool RegisterUser(RegisterModel request)
        {
            var existingUser = _userRL.GetUser(request.Email);
            if (existingUser != null)
                return false;
            var user = _mapper.Map<UserEntity>(request);
            user.Password = PasswordHelper.HashPassword(request.Password);
            _userRL.AddUser(user);
            return true;
        }

        public bool ForgotPassword(string email)
        {
            var user = _userRL.GetUser(email);
            if (user == null) return false; 

            string resetToken = _jwtToken.GenerateResetToken(user.Email);

            string encodedToken = HttpUtility.UrlEncode(resetToken);

            // Construct Reset Link
            
            string subject = "Reset Your Password";
            string body = $"Your reset password Token : {encodedToken}";

            _emailService.SendEmail(user.Email, subject, body);
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
