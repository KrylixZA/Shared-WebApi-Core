# Introduction 
A shared Web API project that contains all the core code necessary to build and secure a Web API in .NET Core.

# Contents
* Global Exception Handler with custom error message implementations.
* Swagger documentation simplification for .NET Core.

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
        .BuildSwaggerServices(services);
    ```
3. In the `Configure` method in `Startup.cs`, add the following line of code:
    ``` C#
    app.UseSwaggerDocs(ApiVersion, ApiTitle);
    ```