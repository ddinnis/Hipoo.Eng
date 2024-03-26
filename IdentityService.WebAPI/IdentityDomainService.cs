using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Domain
{
    public class IdentityDomainService 
    {
        private readonly IIdentityRepository _identityRepository;
        public IdentityDomainService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
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

        public async Task<(SignInResult Result,string？ Toekn)> LoginByPhoneAndPwdAsync(string phoneNum, string password) {
            var result = await CheckPhoneNumAndPwdAsync(phoneNum, password);

            if (result.Succeeded) {
                var user = await _identityRepository
                    .FindByPhoneNumberAsync(phoneNum);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
        }

        private async Task<string> BuildTokenAsync(User? user)
        {
            var roles = await _identityRepository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();

            var claims = new[]
         {
            new Claim(ClaimTypes.Name, "u_admin"), 
            new Claim(ClaimTypes.Role, "r_admin"), 
            new Claim(JwtRegisteredClaimNames.Jti, "admin"),
            new Claim("Username", "Admin"),
            new Claim("Name", "超级管理员")
        };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],     //Issuer
                _configuration["Jwt:Audience"],   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddSeconds(30),    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
