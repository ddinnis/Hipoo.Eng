using IdentityService.Domain;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.WebAPI.Controller.Admin
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IIdentityRepository _repository;
        private readonly IdentityUserManager _userManager;

        public AdminController(IIdentityRepository repository, IdentityUserManager userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
