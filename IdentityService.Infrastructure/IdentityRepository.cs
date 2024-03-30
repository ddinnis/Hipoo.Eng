using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IdentityService.Infrastructure
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly IdentityUserManager _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<IdentityRepository> _logger;

        public IdentityRepository(IdentityUserManager userManager, RoleManager<Role> roleManager, ILogger<IdentityRepository> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            return _userManager.AccessFailedAsync(user);
        }

        public async Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(string userName, string phoneNum)
        {
            if (await FindByNameAsync(userName) != null) {
                return (ErrorResult($"已经存在用户名{userName}"), null, null);
            }
            if (await FindByPhoneNumberAsync(phoneNum) != null)
            {
                return (ErrorResult($"已经存在手机号{phoneNum}"), null, null);
            }
            User user = new User(userName);
            user.PhoneNumber = phoneNum;
            user.PhoneNumberConfirmed = true;
            string password = GeneratePassword();
            var result = await CreateAsync(user, password);
            if (!result.Succeeded) {
                return (result, null, null);
            }
            result = await AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
            {
                return (result, null, null); 
            }

            return (IdentityResult.Success, user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName)) {
                Role role = new Role { Name = roleName };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded) {
                    return result;
                }
            }
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string password)
        {
            if (password.Length < 6) {
                IdentityError error = new IdentityError { Code = "PasswordInvalid", Description = "密码长度不能少于6" };
                return IdentityResult.Failed(error); 
            }
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token,password);
            return resetPasswordResult;
        }

        public async Task<IdentityResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                var error = new IdentityError { Code = "UserNotFound", Description = $"未找到ID为{userId}的用户。" };
                // 使用IdentityResult.Failed方法返回一个包含错误的IdentityResult对象
                return IdentityResult.Failed(error);
            }
            var result = await _userManager.ChangePhoneNumberAsync(user,phoneNum,token);
            if (!result.Succeeded) { 
                await _userManager.AccessFailedAsync(user);
                string errMsg = result.Errors.SumErrors();
                _logger.LogWarning($"{phoneNum}ChangePhoneNumberAsync失败，错误信息{errMsg}");
                return IdentityResult.Success;
            }
            else {
                await ConfirmPhoneNumberAsync(user.Id);//确认手机号
                return IdentityResult.Success;
            }

        }

        public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            if (await _userManager.IsLockedOutAsync(user)) {
                return SignInResult.LockedOut;
            }
            var result = await _userManager.CheckPasswordAsync(user, password);
            if (result)
            {
                return SignInResult.Success;
            }
            else {
                if (lockoutOnFailure)
                {
                    var r = await AccessFailedAsync(user);
                    if (!r.Succeeded)
                    {
                        throw new ApplicationException("AccessFailed failed");
                    }
                }
                return SignInResult.Failed;
            }
        }

        public async Task ConfirmPhoneNumberAsync(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<User?> FindByIdAsync(Guid userId)
        {
            return _userManager.FindByIdAsync(userId.ToString());
        }

        public Task<User?> FindByNameAsync(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public Task<User?> FindByPhoneNumberAsync(string phoneNum)
        {
            return _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNum);
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber)
        {
            return _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return _userManager.GetRolesAsync(user);    
        }

        public async Task<IdentityResult> RemoveUserAsync(Guid id)
        {
            var user = await FindByIdAsync(id);
            // 登录记录存储在 AspNetUserLogins 表中
            var userLoginStore = _userManager.UserLoginStore; //管理用户登录信息的组件
            // 表达了这是一个默认值，意味着在当前上下文中没有特别的取消令牌被提供或需要
            var cancellationToken = default(CancellationToken);
            var logins = await userLoginStore.GetLoginsAsync(user, cancellationToken);
            foreach (var login in logins)
            {
                await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey,cancellationToken);
            }
            user.SoftDelete();
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        //public Task<(IdentityResult, User?, string? password)> RestPasswordAsync(Guid id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task UpdatePhoneNumberAsync(Guid id, string phoneNum)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentException($"用户找不到，id={id}", nameof(id));
            }
            user.PhoneNumber = phoneNum;
            await _userManager.UpdateAsync(user);
        }

        private string GeneratePassword()
        {
            var option = _userManager.Options.Password;
            int length = option.RequiredLength;
            bool nonAlphanumeric = option.RequireNonAlphanumeric;
            bool digit = option.RequireDigit;
            bool lowercase = option.RequireLowercase;
            bool uppercase = option.RequireUppercase;
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);
                password.Append(c);
                if (char.IsDigit(c)) digit = false;
                if (char.IsLower(c)) lowercase = false;
                if (char.IsUpper(c)) uppercase = false;
                if (!char.IsLetterOrDigit(c)) nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));
            return password.ToString();
        }

        private IdentityResult ErrorResult(string msg)
        {
            IdentityError error = new IdentityError { Description = msg };
            return IdentityResult.Failed(error);
        }
    }
}
