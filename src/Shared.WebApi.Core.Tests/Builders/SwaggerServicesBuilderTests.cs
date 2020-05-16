using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Shared.WebApi.Core.Builders;

namespace Shared.WebApi.Core.Tests.Builders
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class SwaggerServicesBuilderTests
    {
        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void WithApiVersion_GivenVersionOne_ShouldSetApiVersion()
        {
            // Arrange
            const int apiVersion = 1;

            var builder = new SwaggerServicesBuilder();

            // Act
            builder.WithApiVersion(apiVersion);

            // Assert
            var property = builder.GetType().GetProperty("ApiVersion", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualVersion = (int)property?.GetValue(builder);
            
            Assert.AreEqual(apiVersion, actualVersion);
        }
        
        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void WithTitle_GivenBasicTitle_ShouldSetTitle()
        {
            // Arrange
            const string apiTitle = "Test API title";

            var builder = new SwaggerServicesBuilder();

            // Act
            builder.WithApiTitle(apiTitle);

            // Assert
            var property = builder.GetType().GetProperty("Title", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = (string)property?.GetValue(builder);
            
            Assert.AreEqual(apiTitle, actualValue);
        }
        
        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void WithDescription_GivenBasicDescription_ShouldSetDescription()
        {
            // Arrange
            const string description = "Test API description";

            var builder = new SwaggerServicesBuilder();

            // Act
            builder.WithApiDescription(description);

            // Assert
            var property = builder.GetType().GetProperty("Description", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = (string)property?.GetValue(builder);
            
            Assert.AreEqual(description, actualValue);
        }
        
        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void WithXmlComments_GivenSingleXmlCommentPath_ShouldAddPathToList()
        {
            // Arrange
            const string path = "/test/path/to/xml/comments.xml";
            var expected = new List<string> { path };

            var builder = new SwaggerServicesBuilder();

            // Act
            builder.WithXmlComments(path);

            // Assert
            var property = builder.GetType().GetProperty("XmlCommentsPaths", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = (List<string>)property?.GetValue(builder);

            actualValue.Should().BeEquivalentTo(expected);
        }
        
        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void WithCoreXmlDocs_GivenTrue_ShouldSetIncludeCoreXmlDocsToTrue()
        {
            // Arrange
            const bool withCoreXml = true;

            var builder = new SwaggerServicesBuilder();

            // Act
            builder.WithCoreXmlDocs(withCoreXml);

            // Assert
            var property = builder.GetType().GetProperty("IncludeCoreXmlDocs", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = (bool)property?.GetValue(builder);
            
            Assert.AreEqual(withCoreXml, actualValue);
        }
        
        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void WithJwtAuthentication_GivenTrue_ShouldSetUseJwtAuthenticationToTrue()
        {
            // Arrange
            const bool useJwtAuth = true;

            var builder = new SwaggerServicesBuilder();

            // Act
            builder.WithJwtAuthentication(useJwtAuth);

            // Assert
            var property = builder.GetType().GetProperty("UseJwtAuthentication", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = (bool)property?.GetValue(builder);
            
            Assert.AreEqual(useJwtAuth, actualValue);
        }

        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void BuildSwaggerServices_GivenWithXmlCommentsTrue_ShouldAddCoreXmlDocsToXmlCommentsList()
        {
            // Arrange
            const bool withCoreXml = true;
            var services = Substitute.For<IServiceCollection>();
            var builder = new SwaggerServicesBuilder();
            builder.WithCoreXmlDocs(withCoreXml);

            // Act
            builder.BuildSwaggerServices(services);

            // Assert
            var property = builder.GetType().GetProperty("XmlCommentsPaths", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = (List<string>)property?.GetValue(builder);

            var match = actualValue?.FirstOrDefault(k => k.Contains("Shared.WebApi.Core.xml"));
            if (match == null)
            {
                Assert.Fail("No core XML documentation included in list");
            }
        }
    }
}