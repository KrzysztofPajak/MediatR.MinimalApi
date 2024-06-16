using MediatR.MinimalApi.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace MediatR.MinimalApi.Helpers;
internal static class EndpointsHelpers
{
    public static async Task<object> CreateRequestAsync(Type requestType, HttpMethod httpMethod, HttpRequest request)
    {
        var result = httpMethod switch
        {
            HttpMethod.Get or HttpMethod.Delete => CreateFromQueryRequest(requestType, request),
            HttpMethod.Post or HttpMethod.Put or HttpMethod.Patch => await CreateFromBodyRequest(requestType, request),
            _ => throw new NotSupportedException($"Http method {httpMethod} is not supported.")
        };

        return result ?? throw new InvalidOperationException("Request cannot be null.");
    }

    private static object? CreateFromQueryRequest(Type requestType, HttpRequest httpRequest)
    {
        var parameterValues = ExtractParameterValues(requestType, httpRequest.Query);
        var request = Activator.CreateInstance(requestType, parameterValues);
        PopulateRequestFromQuery(request, httpRequest.Query);
        return request;
    }

    private static object?[]? ExtractParameterValues(Type requestType, IQueryCollection query)
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
            if (query.TryGetValue(param.Name!, out var value))
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

    private static async ValueTask<object?> CreateFromBodyRequest(Type requestType, HttpRequest httpRequest)
    {
        var request = httpRequest.ContentLength is > 0 ? await httpRequest.ReadFromJsonAsync(requestType) : Activator.CreateInstance(requestType);
        PopulateRequestFromQuery(request, httpRequest.Query);
        return request;
    }

    private static void PopulateRequestFromQuery(object? request, IQueryCollection query)
    {
        if (request == null) return;
        var requestType = request.GetType();

        foreach (var property in requestType.GetProperties())
        {
            if (property.GetCustomAttribute<FromQueryAttribute>() is not null && query.TryGetValue(property.Name, out var value))
            {
                var convertedValue = EndpointsHelpers.ConvertToType(value.ToString(), property.PropertyType);
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
        if (type == typeof(Guid))
        {
            return Guid.Parse(value);
        }
        else if (type == typeof(int))
        {
            return int.Parse(value);
        }
        else if (type == typeof(long))
        {
            return long.Parse(value);
        }
        else if (type == typeof(bool))
        {
            return bool.Parse(value);
        }
        else if (type == typeof(DateTime))
        {
            return DateTime.Parse(value);
        }
        else if (type.IsEnum)
        {
            return Enum.Parse(type, value);
        }
        else
        {
            return Convert.ChangeType(value, type);
        }
    }
}
