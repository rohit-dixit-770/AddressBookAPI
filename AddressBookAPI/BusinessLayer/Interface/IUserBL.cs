using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        bool RegisterUser(RegisterModel request);
        string? LoginUser(LoginModel request);
        bool ForgotPassword(string email);
        bool ResetPassword(string token, string newPassword);
    }
}
