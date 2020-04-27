using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.WebApi.Core.Security
{
    /// <summary>
    /// A custom authorization filter to include the padlock icon on protected API endpoints.
    /// </summary>
    public class AuthOperationFilter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = context.MethodInfo.DeclaringType != null 
                    && (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() 
                    || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()) 
                    && !context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any(); // this excludes methods with AllowAnonymous attribute
            if (!isAuthorized)
            {
                return;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.TryAdd("403", new OpenApiResponse {Description = "Forbidden"});

            var jwtBearerSchema = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearer"
                }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [jwtBearerSchema] = new string[] { }
                }
            };
        }
    }
}