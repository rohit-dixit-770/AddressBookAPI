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

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly JwtTokenHelper _jwtToken;
        private readonly IMapper _mapper;

        public UserBL(IUserRL userRL, JwtTokenHelper jwtToken, IMapper mapper)
        {
            _userRL = userRL;
            _jwtToken = jwtToken;
            _mapper = mapper;
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
    }
}
