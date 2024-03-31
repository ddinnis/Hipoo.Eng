using DotNetCore.CAP;
using IdentityService.Domain;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.Event;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.WebAPI.Controller.Admin
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IIdentityRepository _repository;
        private readonly IdentityUserManager _userManager;
        private readonly ICapPublisher _capBus;
        private readonly ILogger<AdminController> _logger;


        public AdminController(IIdentityRepository repository, IdentityUserManager userManager, ICapPublisher capPublisher, ILogger<AdminController> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _capBus = capPublisher;
            _logger = logger;
        }

        [HttpGet]
        public Task<UserDTO[]> FindAllUsers() {
            return _userManager.Users.Select(u => UserDTO.Create(u)).ToArrayAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<UserDTO> FindById(Guid id) {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return UserDTO.Create(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddAdminUser(AddAdminUserRequest req) {
         (var result,var user, var password ) = await _repository.AddAdminUserAsync(req.UserName,req.PhoneNum);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.SumErrors());
            }

            var userCreatedEvent = new UserCreatedEvent(user.Id, req.UserName, password, req.PhoneNum);
            _logger.LogInformation("IdentityService.User.Created Publish之前");
              _capBus.Publish("IdentityService.User.Created", userCreatedEvent);
            // _eventBus.Publish("IdentityService.User.Created", userCreatedEvent);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAdminUser(Guid id)
        {
            await _repository.RemoveUserAsync(id);
            return Ok();
        }


        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAdminUser(Guid id, EditAdminUserRequest req) 
        {
            var user = await _repository.FindByIdAsync(id);
            if (user == null) return NotFound("用户没找到");
            user.PhoneNumber = req.PhoneNum;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult> ResetAdminUserPassword(Guid id)
        {
            (var result, var user, var password) = await _repository.ResetAdminUserPassword(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.SumErrors());
            }
            //生成的密码短信发给对方
            var eventData = new ResetPasswordEvent(user.Id, user.UserName, password, user.PhoneNumber);
            _capBus.Publish("IdentityService.User.PasswordReset", eventData);
            // eventBus.Publish("IdentityService.User.PasswordReset", eventData);
            return Ok();
        }
    }
}
