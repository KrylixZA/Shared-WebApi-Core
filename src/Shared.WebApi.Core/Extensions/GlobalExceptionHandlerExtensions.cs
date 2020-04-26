using Microsoft.AspNetCore.Builder;
using Shared.WebApi.Core.Exceptions;

namespace Shared.WebApi.Core.Extensions
{
    /// <summary>
    /// A static class containing a variety of extension methods to make global exception handling easier.
    /// </summary>
    public static class GlobalExceptionHandlerExtensions
    {
        /// <summary>
        /// Encapsulates the registration of the global exception handler to simplify use in a project.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}