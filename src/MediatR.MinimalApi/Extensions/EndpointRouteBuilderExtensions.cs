using MediatR.MinimalApi.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MediatR.MinimalApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGetWithMediatR<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IRequest<TResponse>
    {
        return endpoints.MapMethodWithMediatR<TRequest, TResponse>(pattern, endpoints.MapGet);
    }

    public static RouteHandlerBuilder MapPostWithMediatR<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IRequest<TResponse>
    {
        return endpoints.MapMethodWithMediatR<TRequest, TResponse>(pattern, endpoints.MapPost);
    }

    public static RouteHandlerBuilder MapDeleteWithMediatR<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IRequest<TResponse>
    {
        return endpoints.MapMethodWithMediatR<TRequest, TResponse>(pattern, endpoints.MapDelete);
    }

    public static RouteHandlerBuilder MapPatchWithMediatR<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IRequest<TResponse>
    {
        return endpoints.MapMethodWithMediatR<TRequest, TResponse>(pattern, endpoints.MapPatch);
    }

    public static RouteHandlerBuilder MapPutWithMediatR<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IRequest<TResponse>
    {
        return endpoints.MapMethodWithMediatR<TRequest, TResponse>(pattern, endpoints.MapPut);
    }

    private static RouteHandlerBuilder MapMethodWithMediatR<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, Func<string, Delegate, RouteHandlerBuilder> mapMethod)
        where TRequest : IRequest<TResponse>
    {
        return mapMethod(pattern, async ([AsParameters] TRequest request, HttpContext context, [FromServices] IMediator mediator) =>
        {
            try
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            }
            catch (HttpResponseException ex)
            {
                return Results.Problem(statusCode: ex.StatusCode, detail: ex.Message);
            }
        });
    }


}
