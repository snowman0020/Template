using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Template.Domain.DTO;

namespace Template.Helper.Token
{
    public class Token : IToken
    {
        private readonly ILogger<Token> _logger;

        public Token(ILogger<Token> logger)
        {
            _logger = logger;
        }

        public LoginDTO CreateNewToken(string userId, string email, string issuer, int expMinutes, string key)
        {
            _logger.LogInformation($"call: CreateNewToken");

            var result = new LoginDTO();

            _logger.LogDebug($"userId: {userId}, email: {email}, issuer: {issuer}, expiryMinutes: {expMinutes}, key: {key}");

            var refreshToken = Guid.NewGuid().ToString("N");

            var subject = new ClaimsIdentity
            (
                new List<Claim> {
                new Claim("id", userId.ToString()),
                new Claim("name", email.ToString()),
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(expMinutes),
                Subject = subject,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)
            };

            var _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwt = _jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var expires = new TimeSpan(tokenDescriptor.Expires.Value.Ticks - centuryBegin.Ticks).TotalSeconds;

            result.Token = token;
            result.Expires = expires;
            result.RefreshToken = refreshToken;
            result.Email = email;

            _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

            return result;
        } 
    }
}