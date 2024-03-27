using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Common.JWT
{
    public class TokenService : ITokenService
    {
        public string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
        {
            TimeSpan ExpiryDuration = TimeSpan.FromSeconds(options.ExpireSeconds);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(options.Issuer,options.Audience,claims,expires:DateTime.Now.Add(ExpiryDuration),signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
