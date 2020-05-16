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
    public class NotFoundExceptionTests
    {
        [Test]
        public void NotFoundException_GivenNoParams_ShouldCreateBasicBadRequestException()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.NotFound;

            // Act
            var exception = new NotFoundException();

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
        }

        [Test]
        public void NotFoundException_GivenMessageAndErrorCode_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.NotFound;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;

            // Act
            var exception = new NotFoundException(expectedMessage, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
        }
        
        [Test]
        public void NotFoundException_GivenMessageAndErrorCodeAndInnerException_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.NotFound;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;
            var expectedInnerException = new Exception("Test exception");

            // Act
            var exception = new NotFoundException(expectedMessage, expectedInnerException, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
            exception.InnerException.Should().BeEquivalentTo(expectedInnerException);
        }
    }
}