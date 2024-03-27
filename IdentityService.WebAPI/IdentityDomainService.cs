using Common.JWT;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Domain
{
    public class IdentityDomainService 
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        private readonly IOptions<JWTOptions> _optJWT;

        public IdentityDomainService(IIdentityRepository identityRepository, ITokenService tokenService, IOptions<JWTOptions> optJWT)
        {
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _optJWT = optJWT;

        }

        public async Task<SignInResult> CheckUserNameAndPwdAsync(string userName,string Password) {
            var user = await _identityRepository.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var result = await _identityRepository.CheckForSignInAsync(user, Password, true);
            return result;
        }

        public async Task<SignInResult> CheckPhoneNumAndPwdAsync(string phoneNum, string password) {

            var user =await _identityRepository.FindByPhoneNumberAsync(phoneNum);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var result = await _identityRepository.CheckForSignInAsync(user, password,true);
            return result;
        }

        public async Task<(SignInResult Result, string? Toekn)> LoginByPhoneAndPwdAsync(string phoneNum, string password) {
            var result = await CheckPhoneNumAndPwdAsync(phoneNum, password);

            if (result.Succeeded)
            {
                var user = await _identityRepository
                    .FindByPhoneNumberAsync(phoneNum);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            return (result,null);
        }

        public async Task<(SignInResult Result, string? Token)> LoginByUserNameAndPwdAsync(string userName, string password) {
            var checkResult = await CheckUserNameAndPwdAsync(userName, password);
            if (checkResult.Succeeded)
            {
                var user = await _identityRepository.FindByNameAsync(userName);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }



        private async Task<string> BuildTokenAsync(User? user)
        {
            var roles = await _identityRepository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return _tokenService.BuildToken(claims, _optJWT.Value);
        }
    }
}
