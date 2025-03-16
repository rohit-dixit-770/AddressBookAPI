using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBookAPI.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and password management
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class UserAuthenticationController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public UserAuthenticationController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="RegisterModel"></param>
        /// <returns>A response indicating success or failure</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel request)
        {
            bool isRegistered = _userBL.RegisterUser(request);
            if (!isRegistered)
                return BadRequest("User with this email already exists");

            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Logs in a user and generates a JWT token
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns>A JWT token if authentication is successful</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel request)
        {
            var token = _userBL.LoginUser(request);
            if (token == null)
                return Unauthorized("Invalid email or password");

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Initiates the forgot password process by sending a reset email
        /// </summary>
        /// <param name="ForgotPasswordModel"></param>
        /// <returns>A response indicating success or failure</returns>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            bool isSent = _userBL.ForgotPassword(request.Email);
            if (!isSent)
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email not found" });

            return Ok(new ResponseModel<string> { Success = true, Message = "Reset password email sent successfully." });
        }

        /// <summary>
        /// Resets the user's password using a valid reset token
        /// </summary>
        /// <param name="resetPasswordModel"></param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel.NewPassword != resetPasswordModel.ConfirmPassword)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "New Password and Confirm Password are not the same"
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
