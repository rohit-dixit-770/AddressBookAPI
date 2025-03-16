using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.AutoMapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserEntity, UserModel>();
            CreateMap<UserEntity, RegisterModel>().ReverseMap();
            CreateMap<UserEntity, UserModel>().ReverseMap();

        }
    }
}
