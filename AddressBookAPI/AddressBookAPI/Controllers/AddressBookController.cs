using Microsoft.AspNetCore.Mvc;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {


        private readonly ILogger<AddressBookController> _logger;

        public AddressBookController(ILogger<AddressBookController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Api run successful");
            return "API working";
        }
    }
}
