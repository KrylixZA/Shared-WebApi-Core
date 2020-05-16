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
    public class ForbiddenExceptionTests
    {
        [Test]
        public void ForbiddenException_GivenNoParams_ShouldCreateBasicBadRequestException()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Forbidden;

            // Act
            var exception = new ForbiddenException();

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
        }

        [Test]
        public void ForbiddenException_GivenMessageAndErrorCode_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Forbidden;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;

            // Act
            var exception = new ForbiddenException(expectedMessage, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
        }
        
        [Test]
        public void ForbiddenException_GivenMessageAndErrorCodeAndInnerException_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Forbidden;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;
            var expectedInnerException = new Exception("Test exception");

            // Act
            var exception = new ForbiddenException(expectedMessage, expectedInnerException, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
            exception.InnerException.Should().BeEquivalentTo(expectedInnerException);
        }
    }
}