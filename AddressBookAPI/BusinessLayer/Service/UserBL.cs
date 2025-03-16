using System;
using System.Security.Claims;
using System.Web;
using AutoMapper;
using BusinessLayer.Interface;
using Middleware.Email;
using Middleware.Hashing;
using Middleware.JWT;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly JwtTokenHelper _jwtToken;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        private readonly IRedisCacheService _redisCacheService;

        public UserBL(IUserRL userRL, JwtTokenHelper jwtToken, IMapper mapper, EmailService emailService, IRedisCacheService redisCacheService)
        {
            _userRL = userRL;
            _jwtToken = jwtToken;
            _mapper = mapper;
            _emailService = emailService;
            _redisCacheService = redisCacheService;
        }

        public string? LoginUser(LoginModel request)
        {
            var cachedUser = _redisCacheService.GetCache(request.Email);
            UserEntity user;

            if (!string.IsNullOrEmpty(cachedUser))
            {
                user = System.Text.Json.JsonSerializer.Deserialize<UserEntity>(cachedUser);
            }
            else
            {
                user = _userRL.GetUser(request.Email);
                if (user != null)
                {
                    _redisCacheService.SetCache(request.Email, System.Text.Json.JsonSerializer.Serialize(user), 30);
                }
            }

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

            // Store in Redis Cache
            _redisCacheService.SetCache(user.Email, System.Text.Json.JsonSerializer.Serialize(user), 30);

            return true;
        }

        public bool ForgotPassword(string email)
        {
            var user = _userRL.GetUser(email);
            if (user == null) return false;

            string resetToken = _jwtToken.GenerateResetToken(user.Email);

            string encodedToken = HttpUtility.UrlEncode(resetToken);

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

            _redisCacheService.RemoveCache(email);

            return true;
        }
    }
}
