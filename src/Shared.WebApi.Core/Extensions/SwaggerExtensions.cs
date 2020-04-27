using Microsoft.AspNetCore.Builder;

namespace Shared.WebApi.Core.Extensions
{
    /// <summary>
    /// A set of extension methods designed to simplify the use of Swagger documentation.
    /// </summary>
    public static class SwaggerExtensions 
    {
        /// <summary>
        /// Use the given Swagger documentation configuration.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="apiVersion">The API version.</param>
        /// <param name="apiTitle">The API title.</param>
        public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app, int apiVersion, string apiTitle)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v{apiVersion}/swagger.json", $"{apiTitle} API");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}