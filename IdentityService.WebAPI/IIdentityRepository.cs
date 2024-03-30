using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain
{
    public interface IIdentityRepository
    {
        Task<User?> FindByIdAsync(Guid userId);//根据Id获取用户
        Task<User?> FindByNameAsync(string userName);//根据用户名获取用户
        Task<User?> FindByPhoneNumberAsync(string phoneNum);//根据手机号获取用户
        Task<IdentityResult> CreateAsync(User user, string password);//创建用户
        Task<IdentityResult> AccessFailedAsync(User user);//记录一次登陆失败

        Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber);
        Task<IdentityResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token);
        Task<IdentityResult> ChangePasswordAsync(Guid userId, string password);
        Task<IList<string>> GetRolesAsync(User user);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure);
        Task ConfirmPhoneNumberAsync(Guid id);
        Task UpdatePhoneNumberAsync(Guid id, string phoneNum);
        Task<IdentityResult> RemoveUserAsync(Guid id);

        Task<(IdentityResult,User?,string? password)> AddAdminUserAsync(string userName,string phoneNum);
        //Task<(IdentityResult, User?, string? password)> RestPasswordAsync(Guid id);

    }
}
