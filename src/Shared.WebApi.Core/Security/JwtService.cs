using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Shared.WebApi.Core.Security
{
    /// <summary>
    /// A class that provides services for generating JWT tokens.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
  
        /// <summary>
        /// Initializes a new instance of the JWT service.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));  
        }

        /// <inheritdoc />
        public JwtResponse GenerateSecurityToken(string email, DateTime now)
        {
            return GenerateToken(new[]
            {
                new Claim(ClaimTypes.Email, email),
            }, now);
        }

        /// <inheritdoc />
        public JwtResponse GenerateSecurityToken(string email, int userId, DateTime now)
        {
            return GenerateToken(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            }, now);
        }

        /// <inheritdoc />
        public JwtResponse GenerateSecurityToken(string email, Guid userId, DateTime now)
        {
            return GenerateToken(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            }, now);
        }

        private JwtResponse GenerateToken(IEnumerable<Claim> claims, DateTime now)
        {
            var secret = _configuration.GetValue<string>("JwtConfig:Secret");
            var expirationInMinutes = _configuration.GetValue<int>("JwtConfig:ExpirationInMinutes");
            var tokenHandler = new JwtSecurityTokenHandler();  
            var key = Encoding.ASCII.GetBytes(secret);  
            var tokenDescriptor = new SecurityTokenDescriptor  
            {  
                Subject = new ClaimsIdentity(claims),  
                Expires = now.AddMinutes(expirationInMinutes),  
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  
            };  
  
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresInMinutes = expirationInMinutes
            };
        }
    }
}