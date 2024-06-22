using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MediatR.MinimalApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGetWithMediator<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : class
    {
        return endpoints.MapMethodWithMediator<TRequest, TResponse>(pattern, endpoints.MapGet);
    }

    public static RouteHandlerBuilder MapPostWithMediator<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : class
    {
        return endpoints.MapMethodWithMediator<TRequest, TResponse>(pattern, endpoints.MapPost);
    }

    public static RouteHandlerBuilder MapDeleteWithMediator<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : class
    {
        return endpoints.MapMethodWithMediator<TRequest, TResponse>(pattern, endpoints.MapDelete);
    }

    public static RouteHandlerBuilder MapPatchWithMediator<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : class
    {
        return endpoints.MapMethodWithMediator<TRequest, TResponse>(pattern, endpoints.MapPatch);
    }

    public static RouteHandlerBuilder MapPutWithMediator<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : class
    {
        return endpoints.MapMethodWithMediator<TRequest, TResponse>(pattern, endpoints.MapPut);
    }

    private static RouteHandlerBuilder MapMethodWithMediator<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, Func<string, Delegate, RouteHandlerBuilder> mapMethod) 
        where TRequest : class
    {
        return mapMethod(pattern, async ([AsParameters] TRequest request, [FromServices] IMediator mediator) =>
        {
            var response = await mediator.Send(request);
            return Results.Ok(response);
        });
    }


}
