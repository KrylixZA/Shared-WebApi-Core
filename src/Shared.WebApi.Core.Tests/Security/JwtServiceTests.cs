using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
        public void JwtService_GivenNullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange
            const string expectedParamName = "configuration";

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => new JwtService(null!));

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
            var now = DateTime.UtcNow;
            var expectedDateTime = now.AddMilliseconds(-now.Millisecond);

            // Act
            var actualToken = service.GenerateSecurityToken(email, now);

            // Assert
            var token = new JwtSecurityToken(actualToken.AccessToken);
            token.Claims.FirstOrDefault(k => k.Type == ClaimTypes.Email)?.Value.Should().BeEquivalentTo(email);
            token.IssuedAt.Should().BeCloseTo(expectedDateTime);
        }
        
        [Test]
        public void GenerateSecurityToken_GivenEmailAndIntUserId_ShouldCreateExpectedToken()
        {
            // Arrange
            var stubs = GetStubs();
            var service = GetSystemUnderTest(stubs);

            const string email = "test@email.com";
            const int userId = 1;
            var now = DateTime.UtcNow;
            var expectedDateTime = now.AddMilliseconds(-now.Millisecond);

            // Act
            var actualToken = service.GenerateSecurityToken(email, userId, now);

            // Assert
            var token = new JwtSecurityToken(actualToken.AccessToken);
            token.Claims.FirstOrDefault(k => k.Type == ClaimTypes.Email)?.Value.Should().BeEquivalentTo(email);
            token.Claims.FirstOrDefault(k => k.Type == ClaimTypes.NameIdentifier)?.Value.Should().BeEquivalentTo(email);
            token.IssuedAt.Should().BeCloseTo(expectedDateTime);
        }
        
        [Test]
        public void GenerateSecurityToken_GivenEmailAndGuidUserId_ShouldCreateExpectedToken()
        {
            // Arrange
            var stubs = GetStubs();
            var service = GetSystemUnderTest(stubs);

            const string email = "test@email.com";
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expectedDateTime = now.AddMilliseconds(-now.Millisecond);

            // Act
            var actualToken = service.GenerateSecurityToken(email, userId, now);

            // Assert
            var token = new JwtSecurityToken(actualToken.AccessToken);
            token.Claims.FirstOrDefault(k => k.Type == ClaimTypes.Email)?.Value.Should().BeEquivalentTo(email);
            token.Claims.FirstOrDefault(k => k.Type == ClaimTypes.NameIdentifier)?.Value.Should().BeEquivalentTo(email);
            token.IssuedAt.Should().BeCloseTo(expectedDateTime);
        }
    }
}