# [Shared Web API Core](https://github.com/KrylixZA/Shared-WebApi-Core) ![CI](https://github.com/KrylixZA/Shared-WebApi-Core/workflows/CI/badge.svg) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Shared-WebApi-Core&metric=alert_status)](https://sonarcloud.io/dashboard?id=Shared-WebApi-Core)
A shared web API project that contains all the core code necessary to build and secure a web API in .NET Core.

# Contents
* Global Exception Handler with custom error message implementations.
* Swagger documentation simplification for .NET Core.
* JWT security integration with the `[Authorize]` annotation.

# Guides

## Installation
To install this library you can simply reference it as follows:
`dotnet add package Shared.WebApi.Core`

## Global Exception Handler
To utilize the global exception handler, you will need to the following:
1. Create your own implementation of [IErrorMessageSelector](src/Shared.WebApi.Core/Errors/IErrorMessageSelector.cs).
2. Where applicable, create your own custom exception classes that extended [BaseHttpException](src/Shared.WebApi.Core/Exceptions/BaseHttpException.cs).
3. There are also the following exceptions available:
    * [BadRequestException](src/Shared.WebApi.Core/Exceptions/BadRequestException.cs)
    * [ForbiddenException](src/Shared.WebApi.Core/Exceptions/ForbiddenException.cs)
    * [NotFoundException](src/Shared.WebApi.Core/Exceptions/NotFoundException.cs)
    * [UnauthorizedException](src/Shared.WebApi.Core/Exceptions/UnauthorizedException.cs)
4. Register your implementation of `IErrorMessageSelector` in `Startup.cs` in the `ConfigureServices` method as follows:
    ``` C#
    services.TryAddTransient<IErrorMessageSelector, MyErrorMessageSelector>();
    ````
5. Enable Global Exception Handling in the `Configure` method in `Startup.cs` as follows:
    ``` C#
    app.UseGlobalExceptionHandler();
    ```
6. You should now be able to safely throw exceptions anywhere in your code base. 
    * Model validation or route/query parameter exceptions will automatically throw a [BadRequestException](src/Shared.WebApi.Core/Exceptions/BadRequestException.cs).
    * Various authentication exceptions will automatically throw either a [ForbiddenException](src/Shared.WebApi.Core/Exceptions/ForbiddenException.cs) or an [UnauthorizedException](src/Shared.WebApi.Core/Exceptions/UnauthorizedException.cs).

## Swagger Documentation
To use the Swagger documentation, including the XML documents from this library, you will need the following:
1. Define the following constants in your `Startup.cs`:
    ``` C#
    private const string ApiTitle = "My API";
    private const int ApiVersion = 1;
    private const string ApiDescription = "My API description";
    ```
2. In the `ConfigureServices` method in your `Startup.cs`, add the following code:
    ``` C#
    // Set the comments path for the Swagger JSON and UI.
    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);

    // Use the builder to build the Swagger Services.
    new SwaggerServicesBuilder()
        .WithApiTitle(ApiTitle)
        .WithApiVersion(ApiVersion)
        .WithApiDescription(ApiDescription)
        .WithXmlComments(apiXmlPath)
        .WithCoreXmlDocs(true) // Set this to false if you wish to exclude the library comments.
        .WithJwtAuthentication(true) // Set this to false if you are not using JWT authentication.
        .BuildSwaggerServices(services);
    ```
3. In the `Configure` method in `Startup.cs`, add the following line of code:
    ``` C#
    app.UseSwaggerDocs(ApiVersion, ApiTitle);
    ```
4. To enable XML documentation from this library, you will need to modify your API project to include the [Build.Directory.targets](Build.Directory.targets) file. This file effectively copies the XML and PDB files from the various dependency NuGet packages you have referenced to your build directory.
    
## JWT security
1. For the controller actions you wish to protect through JWT security, decorate either the controller or each controller action with the `[Authorize]` annotation.
2. In the `ConfigureServices` method in `Startup.cs`, include token authentication as follows:
    ``` C#
    services.AddTokenAuthentication(Configuration);
    ```
3. In the `Configure` method in `Startup.cs`, include authorization by adding the following line:
    ``` C#
    app.UseAuthorization();
    ```
4. Add a `JwtConfig` section to your `appsettings.json` - similar to the following:
    ``` JSON
    {
        "JwtConfig": {
            "secret": "PDv7DrqznYL6nv7DrqzjnQYO9JxIsWdcjnQYL6nu0f",
            "expirationInMinutes": 60,
            "issuer": "localhost"
        }
    }
    ```
5. Over the controller action that will generate your token (such as a login, etc.), decorate that action with the `[AllowAnonymous]` annotation.
6. For that same controller, add the following depedency to your constructor:
    ``` C#
    public Constructor(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    ```
    Ensure you introduce a private field `private readonly IJwtService _jwtService;`
7. In the controller action that allows anonymous authentication, when you have validated the request, generate and return a token as follows:
    ``` C#
    var token = _jwtService.GenerateSecurityToken("fake@email.com");  
    return Ok(token);
    ```

    In all, your code will look something like this:
    ``` C#
    /// <summary>
    /// A test API to resolve a JWT token.
    /// </summary>
    [Authorize]
    [Route("tokens")]
    public class TestTokensController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Initializes a new instance of the TestTokensController.
        /// </summary>
        /// <param name="jwtService">The JWT service.</param>
        public TestTokensController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }  

        /// <summary>
        /// Generates a random JWT token.
        /// </summary>
        /// <returns>A random JWT token.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetRandomToken()  
        {  
            var token = _jwtService.GenerateSecurityToken("fake@email.com");  
            return Ok(token);
        }
    }
    ```

# Contribute
To contribute to this project, please following the [contributing guide](CONTRIBUTING.md).
