using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure
{
    public class IdentityRepository : IIdentityRepository
    {
        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<(IdentityResult, User?, string? password)> AddAdminUserAsync(string userName, string phoneNum)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ChangePasswordAsync(Guid userId, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ChangePhoneNumAsync(Guid userId, string phoneNum, string token)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmPhoneNumberAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByPhoneNumberAsync(string phoneNum)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> RemoveUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<(IdentityResult, User?, string? password)> RestPasswordAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePhoneNumberAsync(Guid id, string phoneNum)
        {
            throw new NotImplementedException();
        }
    }
}
