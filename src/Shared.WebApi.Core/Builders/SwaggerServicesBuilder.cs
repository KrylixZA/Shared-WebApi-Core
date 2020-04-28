using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Shared.WebApi.Core.Security;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.WebApi.Core.Builders
{
    /// <summary>
    /// A builder class containing a variety of extension methods to make setting up swagger docs much simpler.
    /// </summary>
    public class SwaggerServicesBuilder 
    {
        private int ApiVersion { get; set; }
        private string Title { get; set; }
        private string Description { get; set; }
        private List<string> XmlCommentsPaths { get; }
        private bool IncludeCoreXmlDocs { get; set; }
        private bool UseJwtAuthentication { get; set; }

        /// <summary>
        /// Initializes a new instance of the swagger builder extensions class.
        /// </summary>
        public SwaggerServicesBuilder() 
        {
            XmlCommentsPaths = new List<string>();
        }

        /// <summary>
        /// Sets the API version.
        /// </summary>
        /// <param name="apiVersion">The API version.</param>
        public SwaggerServicesBuilder WithApiVersion(int apiVersion) 
        {
            ApiVersion = apiVersion;
            return this;
        }

        /// <summary>
        /// Sets the API title.
        /// </summary>
        /// <param name="title">The API title.</param>
        public SwaggerServicesBuilder WithApiTitle(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        /// Sets the API description.
        /// </summary>
        /// <param name="description">The API description.</param>
        public SwaggerServicesBuilder WithApiDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Appends to the list of XML comments, the provided XML comments.
        /// </summary>
        /// <param name="xmlCommentPath">The path to the XML comments.</param>
        public SwaggerServicesBuilder WithXmlComments(string xmlCommentPath)
        {
            XmlCommentsPaths.Add(xmlCommentPath);
            return this;
        }

        /// <summary>
        /// Determines whether to include the XML documentation from this library in your Swagger docs.
        /// </summary>
        /// <param name="includeCoreXmlDocs">A boolean indicating whether or not to include the XML documentation.</param>
        public SwaggerServicesBuilder WithCoreXmlDocs(bool includeCoreXmlDocs)
        {
            IncludeCoreXmlDocs = includeCoreXmlDocs;
            return this;
        }

        /// <summary>
        /// Determines whether or not to JWT authentication in the Swagger documentation with the [Authorize] attribute.
        /// </summary>
        /// <param name="useJwtAuthentication">Set to true if you are using JWT authentication.</param>
        public SwaggerServicesBuilder WithJwtAuthentication(bool useJwtAuthentication)
        {
            UseJwtAuthentication = useJwtAuthentication;
            return this;
        }

        /// <summary>
        /// Builds the Swagger services.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void BuildSwaggerServices(IServiceCollection services)
        {
            if (IncludeCoreXmlDocs) 
            {
                var coreXmlComments = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlComments);
                XmlCommentsPaths.Add(coreXmlPath);
            }
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{ApiVersion}", new OpenApiInfo
                {
                    Title = Title, 
                    Version = $"v{ApiVersion}",
                    Description = Description
                });

                if (UseJwtAuthentication)
                {
                    c.OperationFilter<AuthOperationFilter>();
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme.",
                    });
                }

                // Set the comments path for the Swagger JSON and UI.
                foreach (var xmlCommentPath in XmlCommentsPaths) 
                {
                    c.IncludeXmlComments(xmlCommentPath);
                }
            });
        }
    }
}