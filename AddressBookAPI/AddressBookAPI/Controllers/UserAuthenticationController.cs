using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBookAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserAuthenticationController : Controller
    {
        private readonly IUserBL _userBL;

        public UserAuthenticationController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel request)
        {
            bool isRegistered = _userBL.RegisterUser(request);
            if (!isRegistered)
                return BadRequest("User with this email already exists");

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel request)
        {
            var token = _userBL.LoginUser(request);
            if (token == null)
                return Unauthorized("Invalid email or password");

            return Ok(new { Token = token });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            bool isSent = _userBL.ForgotPassword(request.Email);
            if (!isSent)
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email not found" });

            return Ok(new ResponseModel<string> { Success = true, Message = "Reset password email sent successfully." });
        }

        [HttpPost("reset-password")]
        public  IActionResult ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel.NewPassword != resetPasswordModel.ConfirmPassword) 
            {
                return BadRequest(new ResponseModel<string> 
                { 
                    Success = false, 
                    Message = "New Password and Confirm Password are not same" 
                });
            }
                var result = _userBL.ResetPassword(resetPasswordModel.ResetToken, resetPasswordModel.NewPassword);
                return Ok(new ResponseModel<string>
                {
                    Success = result,
                    Message = result ? "Password reset successfully" : "Invalid token or expired",
                });

           
        }

    }
}
