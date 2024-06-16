using MediatR.MinimalApi.Attributes;
using MediatR.MinimalApi.Exceptions;
using MediatR.MinimalApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatR.MinimalApi.Endpoints
{
    internal static class MediatREndpoints
    {

        internal static void MediatREndpoint(this IEndpointRouteBuilder endpoints, IEnumerable<Type> handlerTypes)
        {
            foreach (var handlerType in handlerTypes)
            {
                var attribute = handlerType.GetCustomAttribute<EndpointAttribute>();
                if (attribute == null) continue;

                var route = attribute.Route;
                var httpMethod = attribute.Method;
                var responseType = handlerType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
                

                switch (httpMethod)
                {
                    case Models.HttpMethod.GET:
                        endpoints.MapGet(route, (Delegate)CreateRequestDelegate(handlerType, httpMethod))
                            .AddFiltersFromAttributes(endpoints.ServiceProvider, handlerType)
                            .WithOpenApiDescription(handlerType);
                        break;
                    case Models.HttpMethod.POST:
                        endpoints.MapPost(route, (Delegate)CreateRequestDelegate(handlerType, httpMethod))
                            .AddFiltersFromAttributes(endpoints.ServiceProvider, handlerType)
                            .WithOpenApiDescription(handlerType);
                        break;
                    case Models.HttpMethod.PUT:
                        endpoints.MapPut(route, (Delegate)CreateRequestDelegate(handlerType, httpMethod));
                        break;
                    case Models.HttpMethod.DELETE:
                        endpoints.MapDelete(route, (Delegate)CreateRequestDelegate(handlerType, httpMethod));
                        break;
                    case Models.HttpMethod.PATCH:
                        endpoints.MapPatch(route, (Delegate)CreateRequestDelegate(handlerType, httpMethod));
                        break;
                    default:
                        throw new NotSupportedException($"Http method {httpMethod} is not supported.");
                }
            }
        }


        private static RequestDelegate CreateRequestDelegate(Type requestType, Models.HttpMethod httpMethod)
        {
            return async context =>
            {
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var request = await CreateRequestAsync(requestType, httpMethod, context.Request);

                try
                {
                    var result = await mediator.Send(request);
                    await context.Response.WriteAsJsonAsync(result);
                }
                catch (HttpResponseException ex)
                {
                    await HandleHttpResponseExceptionAsync(context, ex);
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            };
        }
        private static async Task<object> CreateRequestAsync(Type requestType, Models.HttpMethod httpMethod, HttpRequest request)
        {
            var result = httpMethod switch
            {
                Models.HttpMethod.GET or Models.HttpMethod.DELETE => CreateFromQueryRequest(requestType, request),
                Models.HttpMethod.POST or Models.HttpMethod.PUT or Models.HttpMethod.PATCH => await CreateFromBodyRequest(requestType, request),
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
                    args[i] = ConvertToType(value.ToString(), param.ParameterType);
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
                    var convertedValue = ConvertToType(value.ToString(), property.PropertyType);
                    property.SetValue(request, convertedValue);
                }
            }            
        }
        private static object? ConvertToType(string value, Type type)
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

        private static Task HandleHttpResponseExceptionAsync(HttpContext context, HttpResponseException ex)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
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
    }

}