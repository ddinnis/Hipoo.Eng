using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.WebAPI.Controllers.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebAPI.Controller.Login
{
   
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly IIdentityRepository _repository;
        private readonly IdentityDomainService _identityService;


        public LoginController(IIdentityRepository repository, IdentityDomainService identityService)
        {
            _repository = repository;
            _identityService = identityService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Init()
        {
            if (await _repository.FindByNameAsync("admin") != null) {
                return StatusCode((int)HttpStatusCode.Conflict, "已经初始化过了");
            }

            User user = new User("admin");
            var result = await _repository.CreateAsync(user,"123456");
            var token = await _repository.GenerateChangePhoneNumberTokenAsync(user,"15988888888");

            var r = await _repository.ChangePhoneNumAsync(user.Id,"15966666666", token);

            await _repository.AddToRoleAsync(user, "User");
            await _repository.AddToRoleAsync(user, "Admin");

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<LoginVM>> GetUserInfo()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _repository.FindByIdAsync(Guid.Parse(userId));
            if (user == null)//可能用户注销了
            {
                return NotFound();
            }
            var result =  new UserResponse(user.Id,user.UserName, user.PhoneNumber, user.CreationTime);
            return new LoginVM()
            {
                Code = (int)HttpStatusCode.OK,
                Data = result!,
                Message = "获取token成功",
                Ok = true
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req) {
          (var result, string token ) = await _identityService.LoginByPhoneAndPwdAsync(req.PhoneNum, req.Password);

            if (result.Succeeded)
            {
                return token;
            } 
            else if (result.IsLockedOut) {
                return StatusCode((int)HttpStatusCode.Locked, "此账号已经锁定");
            }
            else
            {
                string msg = "登录失败";
                return StatusCode((int)HttpStatusCode.BadRequest, msg);

            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserResponse>> ChangeMyPassword(ChangeMyPasswordRequest req)
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _repository.ChangePasswordAsync(Guid.Parse(userId),req.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else 
            {
                return BadRequest(result.Errors.SumErrors());
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginVM>> LoginByUserNameAndPwd(
    LoginByUserNameAndPwdRequest req)
        {
            (var checkResult, var token) = await _identityService.LoginByUserNameAndPwdAsync(req.UserName, req.Password);
            if (checkResult.Succeeded) {
                return new LoginVM()
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = token!,
                    Message = "获取token成功",
                    Ok = true
                };
            }
            else if (checkResult.IsLockedOut)//尝试登录次数太多
                return StatusCode((int)HttpStatusCode.Locked, "用户已经被锁定");
            else
            {
                string msg = checkResult.ToString();
                return BadRequest("登录失败" + msg);
            }
        }
    }
}
