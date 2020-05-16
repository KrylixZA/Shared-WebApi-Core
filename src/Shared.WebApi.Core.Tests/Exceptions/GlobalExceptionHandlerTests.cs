using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Shared.WebApi.Core.Errors;
using Shared.WebApi.Core.Exceptions;

namespace Shared.WebApi.Core.Tests.Exceptions
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GlobalExceptionHandlerTests
    {
        private struct Stubs
        {
            public RequestDelegate RequestDelegate { get; set; }
            public IErrorMessageSelector ErrorMessageSelector { get; set; }
            public ILogger<GlobalExceptionHandler> Logger { get; set; }
        }

        private static Stubs GetStubs()
        {
            var stubs = new Stubs
            {
                RequestDelegate = Substitute.For<RequestDelegate>(),
                ErrorMessageSelector = Substitute.For<IErrorMessageSelector>(),
                Logger = Substitute.For<ILogger<GlobalExceptionHandler>>()
            };

            return stubs;
        }

        private static GlobalExceptionHandler GetSystemUnderTest(Stubs stubs)
        {
            return new GlobalExceptionHandler(stubs.RequestDelegate, stubs.ErrorMessageSelector, stubs.Logger);
        }

        [Test]
        public void GlobalExceptionHandler_GivenAllParams_ShouldCreateNewInstance()
        {
            // Arrange
            var stubs = GetStubs();

            // Act
            var globalExceptionHandler = new GlobalExceptionHandler(
                stubs.RequestDelegate,
                stubs.ErrorMessageSelector,
                stubs.Logger);

            // Assert
            Assert.IsNotNull(globalExceptionHandler);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void GlobalExceptionHandler_GivenNullRequestDelete_ShouldThrowArgumentNullException()
        {
            // Arrange
            const string expectedParamName = "next";

            var stubs = GetStubs();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => new GlobalExceptionHandler(
                null!,
                stubs.ErrorMessageSelector,
                stubs.Logger));

            // Assert
            Assert.AreEqual(expectedParamName, actual.ParamName);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void GlobalExceptionHandler_GivenNullErrorMessageSelector_ShouldThrowArgumentNullException()
        {
            // Arrange
            const string expectedParamName = "errorMessageSelector";

            var stubs = GetStubs();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => new GlobalExceptionHandler(
                stubs.RequestDelegate,
                null!,
                stubs.Logger));

            // Assert
            Assert.AreEqual(expectedParamName, actual.ParamName);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void GlobalExceptionHandler_GivenNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange
            const string expectedParamName = "logger";

            var stubs = GetStubs();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => new GlobalExceptionHandler(
                stubs.RequestDelegate,
                stubs.ErrorMessageSelector,
                null!));

            // Assert
            Assert.AreEqual(expectedParamName, actual.ParamName);
        }

        [Test]
        public void InvokeAsync_GivenHttpContextAndNoException_ShouldCompleteTask()
        {
            // Arrange
            var stubs = GetStubs();
            var handler = GetSystemUnderTest(stubs);
            var httpContext = Substitute.For<HttpContext>();

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await handler.InvokeAsync(httpContext));
        }

        [Test]
        public async Task InvokeAsync_GivenException_ShouldReturnExpectedErrorResponse()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var expectedErrorResponse = new ErrorResponse
            {
                ErrorCode = -1,
                ErrorDetails = exception.Message,
                InnerExceptions = new[] {exception}
            };

            var stubs = GetStubs();
            stubs.RequestDelegate.Invoke(Arg.Any<HttpContext>()).Throws(exception);
            var handler = GetSystemUnderTest(stubs);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await handler.InvokeAsync(httpContext);

            // Assert
            Assert.AreEqual("application/json", httpContext.Response.ContentType);
            Assert.AreEqual((int) HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var streamText = await reader.ReadToEndAsync();
            var actualErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            Assert.AreEqual(typeof(ErrorResponse), actualErrorResponse.GetType());
            Assert.AreEqual(expectedErrorResponse.ErrorCode, actualErrorResponse.ErrorCode);
            Assert.AreEqual(expectedErrorResponse.ErrorDetails, actualErrorResponse.ErrorDetails);
        }
        
        [Test]
        public async Task InvokeAsync_GivenBadRequestException_ShouldReturnExpectedErrorResponse()
        {
            // Arrange
            var exception = new BadRequestException("Test exception", 1);
            var expectedErrorResponse = new ErrorResponse
            {
                ErrorCode = 1,
                ErrorDetails = exception.Message,
                ErrorMessage = "Test error message",
                InnerExceptions = new[] {exception}
            };

            var stubs = GetStubs();
            stubs.RequestDelegate.Invoke(Arg.Any<HttpContext>()).Throws(exception);
            stubs.ErrorMessageSelector.GetErrorMessage(Arg.Any<int>()).Returns("Test error message");
            var handler = GetSystemUnderTest(stubs);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await handler.InvokeAsync(httpContext);

            // Assert
            Assert.AreEqual("application/json", httpContext.Response.ContentType);
            Assert.AreEqual((int) HttpStatusCode.BadRequest, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var streamText = await reader.ReadToEndAsync();
            var actualErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            Assert.AreEqual(typeof(ErrorResponse), actualErrorResponse.GetType());
            Assert.AreEqual(expectedErrorResponse.ErrorCode, actualErrorResponse.ErrorCode);
            Assert.AreEqual(expectedErrorResponse.ErrorMessage, actualErrorResponse.ErrorMessage);
            Assert.AreEqual(expectedErrorResponse.ErrorDetails, actualErrorResponse.ErrorDetails);
        }
        
        [Test]
        public async Task InvokeAsync_GivenUnauthorizedException_ShouldReturnExpectedErrorResponse()
        {
            // Arrange
            var exception = new UnauthorizedException("Test exception", 1);
            var expectedErrorResponse = new ErrorResponse
            {
                ErrorCode = 1,
                ErrorDetails = exception.Message,
                ErrorMessage = "Test error message",
                InnerExceptions = new[] {exception}
            };

            var stubs = GetStubs();
            stubs.RequestDelegate.Invoke(Arg.Any<HttpContext>()).Throws(exception);
            stubs.ErrorMessageSelector.GetErrorMessage(Arg.Any<int>()).Returns("Test error message");
            var handler = GetSystemUnderTest(stubs);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await handler.InvokeAsync(httpContext);

            // Assert
            Assert.AreEqual("application/json", httpContext.Response.ContentType);
            Assert.AreEqual((int) HttpStatusCode.Unauthorized, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var streamText = await reader.ReadToEndAsync();
            var actualErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            Assert.AreEqual(typeof(ErrorResponse), actualErrorResponse.GetType());
            Assert.AreEqual(expectedErrorResponse.ErrorCode, actualErrorResponse.ErrorCode);
            Assert.AreEqual(expectedErrorResponse.ErrorMessage, actualErrorResponse.ErrorMessage);
            Assert.AreEqual(expectedErrorResponse.ErrorDetails, actualErrorResponse.ErrorDetails);
        }
        
        [Test]
        public async Task InvokeAsync_GivenForbiddenException_ShouldReturnExpectedErrorResponse()
        {
            // Arrange
            var exception = new ForbiddenException("Test exception", 1);
            var expectedErrorResponse = new ErrorResponse
            {
                ErrorCode = 1,
                ErrorDetails = exception.Message,
                ErrorMessage = "Test error message",
                InnerExceptions = new[] {exception}
            };

            var stubs = GetStubs();
            stubs.RequestDelegate.Invoke(Arg.Any<HttpContext>()).Throws(exception);
            stubs.ErrorMessageSelector.GetErrorMessage(Arg.Any<int>()).Returns("Test error message");
            var handler = GetSystemUnderTest(stubs);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await handler.InvokeAsync(httpContext);

            // Assert
            Assert.AreEqual("application/json", httpContext.Response.ContentType);
            Assert.AreEqual((int) HttpStatusCode.Forbidden, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var streamText = await reader.ReadToEndAsync();
            var actualErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            Assert.AreEqual(typeof(ErrorResponse), actualErrorResponse.GetType());
            Assert.AreEqual(expectedErrorResponse.ErrorCode, actualErrorResponse.ErrorCode);
            Assert.AreEqual(expectedErrorResponse.ErrorMessage, actualErrorResponse.ErrorMessage);
            Assert.AreEqual(expectedErrorResponse.ErrorDetails, actualErrorResponse.ErrorDetails);
        }
        
        [Test]
        public async Task InvokeAsync_GivenNotFoundException_ShouldReturnExpectedErrorResponse()
        {
            // Arrange
            var exception = new NotFoundException("Test exception", 1);
            var expectedErrorResponse = new ErrorResponse
            {
                ErrorCode = 1,
                ErrorDetails = exception.Message,
                ErrorMessage = "Test error message",
                InnerExceptions = new[] {exception}
            };

            var stubs = GetStubs();
            stubs.RequestDelegate.Invoke(Arg.Any<HttpContext>()).Throws(exception);
            stubs.ErrorMessageSelector.GetErrorMessage(Arg.Any<int>()).Returns("Test error message");
            var handler = GetSystemUnderTest(stubs);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await handler.InvokeAsync(httpContext);

            // Assert
            Assert.AreEqual("application/json", httpContext.Response.ContentType);
            Assert.AreEqual((int) HttpStatusCode.NotFound, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var streamText = await reader.ReadToEndAsync();
            var actualErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(streamText);
            Assert.AreEqual(typeof(ErrorResponse), actualErrorResponse.GetType());
            Assert.AreEqual(expectedErrorResponse.ErrorCode, actualErrorResponse.ErrorCode);
            Assert.AreEqual(expectedErrorResponse.ErrorMessage, actualErrorResponse.ErrorMessage);
            Assert.AreEqual(expectedErrorResponse.ErrorDetails, actualErrorResponse.ErrorDetails);
        }
    }
}