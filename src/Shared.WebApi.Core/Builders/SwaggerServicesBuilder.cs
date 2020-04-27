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
        private bool UseAuthOperationFilter { get; set; }
        private string SecurityDefinitionName { get; set; }
        private OpenApiSecurityScheme SecurityDefinition { get; set; }

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
        /// Determines whether to use the JWT authorization operation filter or not in your Swagger docs.
        /// </summary>
        /// <param name="useAuthOperationFilter">A boolean indicating whether or not to use the JWT authorization filter.</param>
        public SwaggerServicesBuilder WithOperationFilter(bool useAuthOperationFilter)
        {
            UseAuthOperationFilter = useAuthOperationFilter;
            return this;
        }

        /// <summary>
        /// If a security definition is being used, use this method to set it.
        /// </summary>
        /// <param name="name">The security definition name.</param>
        /// <param name="securityDefinition">The security definition.</param>
        public SwaggerServicesBuilder WithSecurityDefinition(string name, OpenApiSecurityScheme securityDefinition)
        {
            SecurityDefinitionName = name;
            SecurityDefinition = securityDefinition;
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

                if (UseAuthOperationFilter)
                {
                    c.OperationFilter<AuthOperationFilter>();
                }

                if (SecurityDefinition != null)
                {
                    c.AddSecurityDefinition(SecurityDefinitionName, SecurityDefinition);
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