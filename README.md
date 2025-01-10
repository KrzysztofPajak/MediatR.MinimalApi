# MediatR.MinimalApi
![CI](https://github.com/KrzysztofPajak/MediatR.MinimalApi/workflows/.NET/badge.svg)
[![NuGet](https://img.shields.io/nuget/vpre/MediatR.MinimalApi.svg)](https://www.nuget.org/packages/MediatR.MinimalApi)

`MediatR.MinimalApi` is a NuGet package that extends the functionality of minimal APIs by allowing automatic registration of endpoints based on attributes. This library leverages the power of MediatR to simplify and streamline the process of setting up and handling API endpoints. It simplifies the integration of MediatR with Minimal API in ASP.NET Core, allowing you to easily map HTTP endpoints to MediatR handlers, thus enabling clean architecture practices in your application.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Manual Registration](#manual-registration)
    - [Define Request and Response Models](#1-define-request-and-response-models)
    - [Implement the MediatR Request Handler](#2-implement-the-mediatr-request-handler)
    - [Configure Minimal API](#3-configure-minimal-api)
  - [Automatic Registration](#automatic-registration)
    - [Defining Commands/Queries](#1-defining-commandsqueries)
    - [Endpoint Attribute](#2-endpoint-attribute)
    - [Authorization](#3-authorization)
    - [Endpoint Filters](#4-endpoint-filters)
    - [Example](#5-example)
  - [Additional Pipelines](#additional-pipelines)

## Features

- Support for various HTTP methods (GET, POST, DELETE, PATCH, PUT) with MediatR.
- Easy configuration and usage with extension methods.
- Ability to create endpoints through automatic registration using attributes or manual registration
- Compatible with Minimal API in ASP.NET Core 8.
- **Validation Pipeline**: Uses the FluentValidation library to validate your requests.
- **Authorization Pipeline**: Allows you to use the `Authorize` attribute in your `IRequest` classes.
- **Field Access Authorization Pipeline**: Using the `[RoleBasedAccess("Admin")]` attribute ensures that only users with the "Admin" role can access the value of the field.

## Installation

You should install [MediatR with NuGet](https://www.nuget.org/packages/MediatR.MinimalApi):

    Install-Package MediatR.MinimalApi
    
Or via the .NET Core command line interface:

    dotnet add package MediatR.MinimalApi
    
## Usage

### Manual Registration

#### 1. Define Request and Response Models
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
#### 2. Implement the MediatR Request Handler

#### 3. Configure Minimal API
Configure Minimal API to use MediatR and the extension methods from the MediatR.MinimalApi package:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<YourRequestHandler>());

var app = builder.Build();

// Define endpoints
app.MapGetWithMediatR<YourGetRequest, YourResponse>("/example-get");
app.MapGetWithMediatR<YourGetWithQueryParamRequest, YourResponse>("/example-getbyid");
app.MapPostWithMediatR<YourPostRequest, YourResponse>("/example-post");
app.MapPostWithMediatR<YourPostWithQueryParamRequest, YourResponse>("/example-post-param");
app.MapDeleteWithMediatR<YourDeleteRequest, YourResponse>("/example-delete");
app.MapPatchWithMediatR<YourPatchRequest, YourResponse>("/example-patch");
app.MapPutWithMediatR<YourPutRequest, YourResponse>("/example-put");

app.Run();
```

### Automatic Registration

After installing the MediatR.MinimalApi package, you can automatically register all your Commands/Queries with MediatR by using the following code in your Program.cs:
```csharp
app.MapMediatREndpoints(typeof(Program).Assembly);
```
#### 1. Defining Commands/Queries
To ensure each endpoint is registered, you need to define your commands with the Endpoint attribute. Here’s an example:
```csharp
[Endpoint("user/create", HttpMethod.Post, "User")]
public class CreateUserCommand : IRequest<User>
```

#### 2. Endpoint Attribute
The Endpoint attribute is used to define the route, HTTP method, and tag for OpenAPI documentation. Here’s the structure of the Endpoint attribute:

```csharp
[Endpoint("route", HttpMethod.Method, "tag")]
```
 - route: The route of the endpoint.
 - HttpMethod.Method: The HTTP method for the endpoint (HttpMethod.Get, HttpMethod.Post, HttpMethod.Delete, HttpMethod.Patch, HttpMethod.Put).
 - Tag: A tag for OpenAPI documentation.

#### 3. Authorization
If you need to secure your endpoint with authorization, you can use the `Authorize` attribute:
```csharp
[Authorize]
[Endpoint("user/create", HttpMethod.Post, "User")]
public class CreateUserCommand : IRequest<User>
```

#### 4. Endpoint Filters
If you need to apply filters to your endpoint, use the `EndpointFilter` attribute:
```csharp
[Authorize]
[EndpointFilter<CustomFilter>()]
[Endpoint("user/create", HttpMethod.Post, "User")]
public class CreateUserCommand : IRequest<User>
```
#### 5. Example
Here is a complete example of defining and registering an endpoint with authorization and a custom filter:

```csharp
[Authorize]
[EndpointFilter<CustomFilter>()]
[Endpoint("user/create", HttpMethod.Post, "User")]
public class CreateUserCommand : IRequest<User>
{
    // Command properties
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    public Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Handler logic
    }
}

public class CustomFilter : IEndpointFilter
{
   ...
}
```
In your `Program.cs`, you would register the endpoints like this:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
...
app.MapMediatREndpoints(typeof(Program).Assembly);

app.Run();
```
### Additional Pipelines

By registering the MediatR extension in `Program.cs` using:

```csharp
builder.Services.MinimalApiMediatRExtensions();
```

You enable additional pipelines:

1. **Validation Pipeline**: Uses the FluentValidation library to validate your requests.
2. **Authorization Pipeline**: Allows you to use the `Authorize` attribute in your `IRequest` classes.

#### Example of Authorization Attribute
```csharp
 [Authorize]
 public record CreateCompanyCommand([FromBody] CreateCompanyRequest Role) : IRequest<Company>;
```

3. **Field Access Authorization Pipeline**: Using the `[RoleBasedAccess("Admin")]` attribute ensures that only users with the "Admin" role can access the value of the field.
#### Example of Field Access Authorization

You can use the `[RoleBasedAccess("Admin")]` attribute to restrict access to specific fields based on user roles. Here’s an example:

```csharp
public class YourResponse
{
    public string Result { get; set; }

    [RoleBasedAccess("Admin")]
    public string AdminOnlyField { get; set; }
}
```
