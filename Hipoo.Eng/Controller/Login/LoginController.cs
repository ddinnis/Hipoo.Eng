using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityService.WebAPI.Controller.Login
{
   
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public ActionResult<User> Get()
        {
            _logger.LogInformation("Hello Start");
            var user = new User("Hello");
            _logger.LogInformation("Hello End" + user.Email+ user.UserName + user.CreationTime);

            return Ok(user);
        }
    }
}
