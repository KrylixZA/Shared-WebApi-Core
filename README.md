# Introduction 
A shared Web API project that contains all the core code necessary to build and secure a Web API in .NET Core.

# Contents
* Global Exception Handler with custom error message implementations.
* JWT security authorization for controller endpoints.

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