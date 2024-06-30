using MediatR.MinimalApi.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace MediatR.MinimalApi.Helpers;
internal static class EndpointsHelpers
{
    public static async ValueTask<object> CreateRequestAsync(Type requestType, HttpMethod httpMethod, HttpRequest request)
    {
        var result = httpMethod switch
        {
            HttpMethod.Get or HttpMethod.Delete => CreateFromQueryRequest(requestType, request),
            HttpMethod.Post or HttpMethod.Put or HttpMethod.Patch => await CreateFromBodyRequest(requestType, request),
            _ => throw new NotSupportedException($"Http method {httpMethod} is not supported.")
        };

        return result ?? throw new InvalidOperationException("Request cannot be null.");
    }
    private static async ValueTask<object?> CreateFromBodyRequest(Type requestType, HttpRequest httpRequest)
    {
        httpRequest.EnableBuffering();
        httpRequest.Body.Position = 0;
        var request = httpRequest.ContentLength is > 0 ? await httpRequest.ReadFromJsonAsync(requestType) : Activator.CreateInstance(requestType);
        PopulateRequestFromQuery(request, httpRequest);
        return request;
    }

    private static object? CreateFromQueryRequest(Type requestType, HttpRequest httpRequest)
    {
        var parameterValues = ExtractParameterValues(requestType, httpRequest);
        var request = Activator.CreateInstance(requestType, parameterValues);
        PopulateRequestFromQuery(request, httpRequest);
        return request;
    }

    private static object?[]? ExtractParameterValues(Type requestType, HttpRequest httpRequest)
    {
        var constructor = requestType.GetConstructors().FirstOrDefault();
        if (constructor == null)
        {
            return default;
        }

        var parameters = constructor.GetParameters();
        var args = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            if(httpRequest.RouteValues.TryGetValue(param.Name!, out var routeValue))
            {
                args[i] = EndpointsHelpers.ConvertToType(routeValue!.ToString()!, param.ParameterType);
            }
            else if (httpRequest.Query.TryGetValue(param.Name!, out var value))
            {
                args[i] = EndpointsHelpers.ConvertToType(value.ToString(), param.ParameterType);
            }
            else
            {
                args[i] = GetDefaultValue(param.ParameterType);
            }
        }

        return args;
    }
    private static object? GetDefaultValue(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    private static void PopulateRequestFromQuery(object? request, HttpRequest httpRequest)
    {
        if (request == null) return;
        var requestType = request.GetType();

        foreach (var property in requestType.GetProperties())
        {
            if (property.GetCustomAttribute<FromQueryAttribute>() is not null && httpRequest.Query.TryGetValue(property.Name, out var value))
            {
                var convertedValue = EndpointsHelpers.ConvertToType(value.ToString(), property.PropertyType);
                property.SetValue(request, convertedValue);
            }
            if (property.GetCustomAttribute<FromRouteAttribute>() is not null && httpRequest.RouteValues.TryGetValue(property.Name, out var routeValue))
            {
                var convertedValue = EndpointsHelpers.ConvertToType(routeValue!.ToString()!, property.PropertyType);
                property.SetValue(request, convertedValue);
            }
        }
    }

    public static Task HandleHttpResponseExceptionAsync(HttpContext context, HttpResponseException ex)
    {
        var problemDetails = new ProblemDetails
        {
            Status = ex.StatusCode,
            Title = "An error occurred while processing your request.",
            Detail = ex.Message
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = ex.StatusCode;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    public static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = ex.Message,
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    public static object? ConvertToType(string value, Type type)
    {
        return type switch
        {
            _ when type == typeof(Guid) => Guid.Parse(value),
            _ when type == typeof(int) => int.Parse(value),
            _ when type == typeof(long) => long.Parse(value),
            _ when type == typeof(bool) => bool.Parse(value),
            _ when type == typeof(DateTime) => DateTime.Parse(value),
            _ when type.IsEnum => Enum.Parse(type, value),
            _ => Convert.ChangeType(value, type),
        };
    }

}
