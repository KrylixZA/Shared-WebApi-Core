using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Shared.WebApi.Core.Builders
{
    /// <summary>
    /// A builder class containing a variety of extension methods to make setting up swagger docs much simpler.
    /// </summary>
    public class SwaggerServicesBuilder 
    {
        private readonly ILogger<SwaggerServicesBuilder> _logger;
        private int ApiVersion { get; set; }
        private string Title { get; set; }
        private string Description { get; set; }
        private IEnumerable<string> XmlCommentsPaths { get; set; }

        /// <summary>
        /// Initializes a new instance of the swagger builder extensions class.
        /// </summary>
        public SwaggerServicesBuilder(ILogger<SwaggerServicesBuilder> logger) 
        {
            _logger = logger;
            XmlCommentsPaths = new List<string>();
            var coreXmlComments = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlComments);
            _ = XmlCommentsPaths.Append(coreXmlPath);
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
            _ = XmlCommentsPaths.Append(xmlCommentPath);
            return this;
        }

        /// <summary>
        /// Builds the Swagger services.
        /// </summary>
        /// <param name="services"></param>
        public void BuildSwaggerServices(IServiceCollection services)
        {
            _logger.LogTrace("Registering Swagger Gen with the following config:");
            _logger.LogTrace($"API version: v{ApiVersion}");
            _logger.LogTrace($"API title: {Title}");
            _logger.LogTrace($"API description: {Description}");
            _logger.LogTrace($"Xml Comment Files: {JsonConvert.SerializeObject(XmlCommentsPaths)}");
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{ApiVersion}", new OpenApiInfo
                {
                    Title = Title, 
                    Version = $"v{ApiVersion}",
                    Description = Description
                });
                
                // Set the comments path for the Swagger JSON and UI.
                foreach (var xmlCommentPath in XmlCommentsPaths) 
                {
                    c.IncludeXmlComments(xmlCommentPath);
                }
            });
        }
    }
}