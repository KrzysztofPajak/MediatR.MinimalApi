# MediatR.MinimalApi (Not Yet Available)

`MediatR.MinimalApi` is a NuGet package that simplifies the integration of MediatR with Minimal API in ASP.NET Core. This package allows you to easily map HTTP endpoints to MediatR handlers, enabling clean architecture practices in your application.

## Features

- Support for various HTTP methods (GET, POST, DELETE, PATCH, PUT) with MediatR.
- Easy configuration and usage with extension methods.
- Compatible with Minimal API in ASP.NET Core.

## Installation

> The `MediatR.MinimalApi` package is not yet published on NuGet. Once available, you can install it using the following command:

```bash
dotnet add package MediatR.MinimalApi
```

## Usage
1. Define Request and Response Models
Create your request and response models that will be handled by MediatR:

```csharp
public class YourGetRequest : IRequest<YourResponse>
{
}
public class YourGetWithQueryParamRequest : IRequest<YourResponse>
{
    [FromQuery]
    public int Id { get; set; }
}
public class YourPostRequest : IRequest<YourResponse>
{
    [FromBody]
    public Test Data { get; set; }

    public class Test
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
    }
}
public class YourPostWithQueryParamRequest : IRequest<YourResponse>
{
    [FromBody]
    public Test Data { get; set; }

    [FromQuery]
    public int Id { get; set; }

    public class Test
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
    }
}
public class YourResponse
{
    public string Result { get; set; }
}

```
2. Implement the MediatR Request Handler

3. Configure Minimal API
Configure Minimal API to use MediatR and the extension methods from the MediatR.MinimalApi package:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<YourRequestHandler>());

var app = builder.Build();

// Define endpoints
app.MapGetWithMediator<YourGetRequest, YourResponse>("/example-get");
app.MapGetWithMediator<YourGetWithQueryParamRequest, YourResponse>("/example-getbyid");
app.MapPostWithMediator<YourPostRequest, YourResponse>("/example-post");
app.MapPostWithMediator<YourPostWithQueryParamRequest, YourResponse>("/example-post-param");
app.MapDeleteWithMediator<YourDeleteRequest, YourResponse>("/example-delete");
app.MapPatchWithMediator<YourPatchRequest, YourResponse>("/example-patch");
app.MapPutWithMediator<YourPutRequest, YourResponse>("/example-put");

app.Run();
```
