# Shared Web API Core [![Build Status](https://dev.azure.com/headleysj/Source%20Code/_apis/build/status/Shared-WebApi-Core?branchName=master)](https://dev.azure.com/headleysj/Source%20Code/_build/latest?definitionId=11&branchName=master) [![Shared.WebApi.Core package in headleysj feed in Azure Artifacts](https://feeds.dev.azure.com/headleysj/_apis/public/Packaging/Feeds/404449e0-6d24-4a4e-bc3e-4634d3f54a5a/Packages/951922b0-f675-434e-ba67-1249b76eedde/Badge)](https://dev.azure.com/headleysj/Source%20Code/_packaging?_a=package&feed=404449e0-6d24-4a4e-bc3e-4634d3f54a5a&package=951922b0-f675-434e-ba67-1249b76eedde&preferRelease=true) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Shared-WebApi-Core&metric=alert_status)](https://sonarcloud.io/dashboard?id=Shared-WebApi-Core)
A shared web API project that contains all the core code necessary to build and secure a web API in .NET Core.

# Contents
* Global Exception Handler with custom error message implementations.
* Swagger documentation simplification for .NET Core.
* JWT security integration with the `[Authorize]` annotation.

# Guides
## Global Exception Handler
To utilize the global exception handler, you will need to the following:
1. Create your own implementation of [IErrorMessageSelector](src/Shared.WebApi.Core/Errors/IErrorMessageSelector.cs).
2. Register your implementation of `IErrorMessageSelector` in `Startup.cs` in the `ConfigureServices` method as follows:
    ``` C#
    services.TryAddTransient<IErrorMessageSelector, MyErrorMessageSelector>();
    ````
3. Enable Global Exception Handling in the `Configure` method in `Startup.cs` as follows:
    ``` C#
    app.UseGlobalExceptionHandler();
    ```

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
4. To enable XML documentation from this library, modify your `.csproj` file with the following:
    
    4.1 In the `<ItemGroup></ItemGroup>` list that contains your package references, where you are referencing this package, modify it to something to the following:
    ``` XML
    <PackageReference Include="Shared.WebApi.Core" Version="1.0.0.57" GeneratePathProperty="true" />
    ```
    [Click here](https://blog.dangl.me/archive/accessing-nuget-package-paths-in-your-net-sdk-based-csproj-files/) to read more about `GeneratePathProperty`.

    4.2 Add the following post-build event to your project:
    ``` XML
    <Target Name="PostBuild" AfterTargets="PostBuildEvent">
        <Exec Command="cp &quot;$(PkgShared_WebApi_Core)/lib/netcoreapp3.1/Shared.WebApi.Core.xml&quot; &quot;$(ProjectDir)bin/$(Configuration)/$(TargetFramework)/&quot;" Condition=" '$(OS)' == 'Unix' " />
        <Exec Command="xcopy /Y /I /E &quot;$(PkgShared_WebApi_Core)\lib\netcoreapp3.1\Shared.WebApi.Core.xml&quot; &quot;$(ProjectDir)bin\$(Configuration)\$(TargetFramework)\&quot;" Condition=" '$(OS)' == 'Windows_NT' " />
    </Target>
    ```
    This will copy the XML documentation from this project, with whichever version you have installed, to the build output directory of your project. Swagger will now be able to pick up the XML files and your docs will include any classes from this package as well.

    If you do not wish to use this, pass in a false value for the `includeCoreXmlDocs` paramter in the `BuildSwaggerServices` method. This is the default value as well.

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
