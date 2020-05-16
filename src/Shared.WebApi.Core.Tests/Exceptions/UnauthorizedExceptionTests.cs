using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Shared.WebApi.Core.Exceptions;

namespace Shared.WebApi.Core.Tests.Exceptions
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class UnauthorizedExceptionTests
    {
        [Test]
        public void UnauthorizedException_GivenNoParams_ShouldCreateBasicBadRequestException()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var exception = new UnauthorizedException();

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
        }

        [Test]
        public void UnauthorizedException_GivenMessageAndErrorCode_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;

            // Act
            var exception = new UnauthorizedException(expectedMessage, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
        }
        
        [Test]
        public void UnauthorizedException_GivenMessageAndErrorCodeAndInnerException_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;
            var expectedInnerException = new Exception("Test exception");

            // Act
            var exception = new UnauthorizedException(expectedMessage, expectedInnerException, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
            exception.InnerException.Should().BeEquivalentTo(expectedInnerException);
        }
    }
}