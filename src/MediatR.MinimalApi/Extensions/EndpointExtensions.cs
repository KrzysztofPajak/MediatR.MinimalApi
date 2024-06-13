using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Routing;
using System.Reflection;
using MediatR.MinimalApi.Endpoints;

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
            .Where(t => t.GetCustomAttributes<EndpointAttribute>().Any() && ImplementsIRequest(t)).ToArray();

        if (endpointHandlers.Length == 0) return;        

        endpoints.MediatREndpoint(endpointHandlers);

    }

    private static bool ImplementsIRequest(Type type)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
    }
}
