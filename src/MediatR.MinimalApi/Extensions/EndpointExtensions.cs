using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace MediatR.MinimalApi.Extensions;

public static class EndpointExtensions
{

    public static void MapMediatREndpoints(this IEndpointRouteBuilder endpoints, params Assembly[]? assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [Assembly.GetExecutingAssembly()];
        }

        var endpointHandlers = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttributes<EndpointAttribute>().Any() && ImplementsIRequest(t));

        foreach (var requestType in endpointHandlers)
        {
            var attribute = requestType.GetCustomAttribute<EndpointAttribute>();
            var route = attribute!.Route;
            var httpMethod = attribute!.Method;

            var responseType = requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];

            switch (attribute.Method)
            {
                case Models.HttpMethod.GET:
                    endpoints.MapGet(route, requestType, responseType);
                    break;
                case Models.HttpMethod.POST:
                    endpoints.MapPost(route, requestType, responseType);
                    break;
                case Models.HttpMethod.PUT:
                    endpoints.MapPut(route, requestType, responseType);
                    break;
                case Models.HttpMethod.DELETE:
                    endpoints.MapDelete(route, requestType, responseType);
                    break;
                case Models.HttpMethod.PATCH:
                    endpoints.MapPatch(route, requestType, responseType);
                    break;
                default:
                    throw new NotSupportedException($"Http method {httpMethod} is not supported.");
            }
        }
    }

    private static bool ImplementsIRequest(Type type)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
    }
}
