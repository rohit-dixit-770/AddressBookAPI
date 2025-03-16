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

    }
}
