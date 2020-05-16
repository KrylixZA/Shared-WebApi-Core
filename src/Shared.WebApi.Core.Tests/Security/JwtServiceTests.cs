using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NUnit.Framework;
using Shared.WebApi.Core.Security;

namespace Shared.WebApi.Core.Tests.Security
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class JwtServiceTests
    {
        private struct Stubs
        {
            public IConfiguration Configuration { get; set; }
        }

        private static Stubs GetStubs()
        {
            var mockAppSettings = new Dictionary<string, string>
            {
                {
                    "JwtConfig:Secret", Guid.NewGuid().ToString()
                },
                {
                    "JwtConfig:ExpirationInMinutes", "60"
                }
            };

            var stubs = new Stubs
            {
                Configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(mockAppSettings)
                    .Build()
            };

            return stubs;
        }

        private static JwtService GetSystemUnderTest(Stubs stubs)
        {
            return new JwtService(stubs.Configuration);
        }

        private static string GenerateToken(IEnumerable<Claim> claims, Stubs stubs)
        {
            var secret = stubs.Configuration.GetValue<string>("JwtConfig:Secret");
            var expirationInMinutes = stubs.Configuration.GetValue<int>("JwtConfig:ExpirationInMinutes");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Test]
        public void JwtService_GivenAllParams_ShouldCreateNewInstance()
        {
            // Arrange
            var stubs = GetStubs();

            // Act
            var service = new JwtService(stubs.Configuration);

            // Assert
            Assert.IsNotNull(service);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void JwtService_GivenNullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange
            const string expectedParamName = "configuration";

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => new JwtService(null));

            // Assert
            Assert.AreEqual(expectedParamName, actual.ParamName);
        }

        [Test]
        public void GenerateSecurityToken_GivenEmail_ShouldCreateExpectedToken()
        {
            // Arrange
            var stubs = GetStubs();
            var service = GetSystemUnderTest(stubs);

            const string email = "test@email.com";

            var expectedToken = new JwtResponse
            {
                AccessToken = GenerateToken(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                }, stubs),
                ExpiresInMinutes = 60
            };

            // Act
            var actualToken = service.GenerateSecurityToken(email);

            // Assert
            actualToken.Should().BeEquivalentTo(expectedToken);
        }
        
        [Test]
        public void GenerateSecurityToken_GivenEmailAndIntUserId_ShouldCreateExpectedToken()
        {
            // Arrange
            var stubs = GetStubs();
            var service = GetSystemUnderTest(stubs);

            const string email = "test@email.com";
            const int userId = 1;

            var expectedToken = new JwtResponse
            {
                AccessToken = GenerateToken(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, stubs),
                ExpiresInMinutes = 60
            };

            // Act
            var actualToken = service.GenerateSecurityToken(email, userId);

            // Assert
            actualToken.Should().BeEquivalentTo(expectedToken);
        }
        
        [Test]
        public void GenerateSecurityToken_GivenEmailAndGuidUserId_ShouldCreateExpectedToken()
        {
            // Arrange
            var stubs = GetStubs();
            var service = GetSystemUnderTest(stubs);

            const string email = "test@email.com";
            var userId = Guid.NewGuid();

            var expectedToken = new JwtResponse
            {
                AccessToken = GenerateToken(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, stubs),
                ExpiresInMinutes = 60
            };

            // Act
            var actualToken = service.GenerateSecurityToken(email, userId);

            // Assert
            actualToken.Should().BeEquivalentTo(expectedToken);
        }
    }
}