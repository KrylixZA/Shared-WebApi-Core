using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.WebApi.Core.Errors;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// A global exception handler middleware to handle errors on the API and generate valid error response.
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IErrorMessageSelector _errorMessageSelector;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the GlobalExceptionHandler class.
        /// </summary>
        /// <param name="next">The next response.</param>
        /// <param name="errorMessageSelector">The error message selector.</param>
        /// <param name="logger">The logger.</param>
        public GlobalExceptionHandler(
            RequestDelegate next,
            IErrorMessageSelector errorMessageSelector,
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
            _next = next;
            _errorMessageSelector = errorMessageSelector;
        }

        /// <summary>
        /// Invokes the action asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var errorResponse = new ErrorResponse
            {
                ErrorDetails = exception.Message,
                InnerExceptions = new[]
                {
                    exception
                }
            };

            if (exception is BaseHttpException httpException)
            {
                context.Response.StatusCode = (int) httpException.GetHttpStatusCode();
                errorResponse.ErrorMessage = _errorMessageSelector.GetErrorMessage(httpException.ErrorCode);
                errorResponse.ErrorCode = httpException.ErrorCode;
            }
            
            return context.Response.WriteAsync(errorResponse.ToString());
        }
    }
}