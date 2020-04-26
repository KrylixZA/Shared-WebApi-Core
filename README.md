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
    new SwaggerServicesBuilder(includeCoreXmlDocs = true)
        .WithApiTitle(ApiTitle)
        .WithApiVersion(ApiVersion)
        .WithApiDescription(ApiDescription)
        .WithXmlComments(apiXmlPath)
        .BuildSwaggerServices(services);
    // Set includeCoreXmlDocs = false if you do not wish to use the XML documentation from this library.
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
        <Exec Command="cp &quot;$(PkgShared_WebApi_Core)/lib/netstandard2.1/Shared.WebApi.Core.xml&quot; &quot;$(ProjectDir)bin/$(Configuration)/$(TargetFramework)/&quot;" Condition=" '$(OS)' == 'Unix' " />
        <Exec Command="xcopy /Y /I /E &quot;$(PkgShared_WebApi_Core)\lib\netstandard2.1\Shared.WebApi.Core.xml&quot; &quot;$(ProjectDir)bin\$(Configuration)\$(TargetFramework)\&quot;" Condition=" '$(OS)' == 'Windows_NT' " />
    </Target>
    ```
    This will copy the XML documentation from this project, with whichever version you have installed, to the build output directory of your project. Swagger will now be able to pick up the XML files and your docs will include any classes from this package as well.

    If you do not wish to use this, simply create the `SwaggerServicesBuilder` class, passing in a false value for the `includeCoreXmlDocs` variable. This is the default value as well.