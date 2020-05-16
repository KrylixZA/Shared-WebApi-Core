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
    public class BadRequestExceptionTests
    {
        [Test]
        public void BadRequestException_GivenNoParams_ShouldCreateBasicBadRequestException()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.BadRequest;

            // Act
            var exception = new BadRequestException();

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
        }

        [Test]
        public void BadRequestException_GivenMessageAndErrorCode_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.BadRequest;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;

            // Act
            var exception = new BadRequestException(expectedMessage, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
        }
        
        [Test]
        public void BadRequestException_GivenMessageAndErrorCodeAndInnerException_ShouldSetExpectedProperties()
        {
            // Arrange
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.BadRequest;
            const string expectedMessage = "Test error message";
            const int expectedErrorCode = 1;
            var expectedInnerException = new Exception("Test exception");

            // Act
            var exception = new BadRequestException(expectedMessage, expectedInnerException, expectedErrorCode);

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, exception.GetHttpStatusCode());
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
            exception.InnerException.Should().BeEquivalentTo(expectedInnerException);
        }
    }
}