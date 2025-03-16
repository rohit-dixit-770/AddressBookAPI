using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        UserEntity GetUser(string email);
        void AddUser(UserEntity user);
        void UpdateUser(UserEntity user);
    }
}
