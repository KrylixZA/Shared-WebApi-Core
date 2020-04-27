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
            _configuration = configuration;  
        }

        /// <inheritdoc />
        public JwtResponse GenerateSecurityToken(string email)
        {
            return GenerateToken(new[]
            {
                new Claim(ClaimTypes.Email, email),
            });
        }

        /// <inheritdoc />
        public JwtResponse GenerateSecurityToken(string email, int userId)
        {
            return GenerateToken(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            });
        }

        /// <inheritdoc />
        public JwtResponse GenerateSecurityToken(string email, Guid userId)
        {
            return GenerateToken(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            });
        }

        private JwtResponse GenerateToken(IEnumerable<Claim> claims)
        {
            var secret = _configuration.GetSection("JwtConfig").GetSection("secret").Value;
            var expirationInMinutes = _configuration.GetSection("JwtConfig").GetSection("expirationInMinutes").Value;
            var tokenHandler = new JwtSecurityTokenHandler();  
            var key = Encoding.ASCII.GetBytes(secret);  
            var tokenDescriptor = new SecurityTokenDescriptor  
            {  
                Subject = new ClaimsIdentity(claims),  
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(expirationInMinutes)),  
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  
            };  
  
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = int.Parse(expirationInMinutes)
            };
        }
    }
}