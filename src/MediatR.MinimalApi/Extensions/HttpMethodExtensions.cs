using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Mime;

namespace MediatR.MinimalApi.Extensions;

internal static class HttpMethodExtensions
{
    public static void MapGet(this IEndpointRouteBuilder endpoints, string route, Type requestType, Type responseType)
    {
        endpoints.MapGet(route, async (IMediator mediator, HttpContext context) =>
        {
            var request = CreateAndPopulateRequest(requestType, context.Request.Query);
            var result = await mediator.Send(request!);
            await context.Response.WriteAsJsonAsync(result);
        }).Produces(StatusCodes.Status200OK, responseType)
        .WithOpenApiDescription(requestType);
    }

    public static void MapPost(this IEndpointRouteBuilder endpoints, string route, Type requestType, Type responseType)
    {
        endpoints.MapPost(route, async (IMediator mediator, HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync(requestType);
            var result = await mediator.Send(request!);
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsJsonAsync(result);
        }).Accepts(requestType, MediaTypeNames.Application.Json)
        .Produces(StatusCodes.Status200OK, responseType); 
    }
    public static void MapPut(this IEndpointRouteBuilder endpoints, string route, Type requestType, Type responseType)
    {
        endpoints.MapPut(route, async (IMediator mediator, HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync(requestType);
            var result = await mediator.Send(request!);
            await context.Response.WriteAsJsonAsync(result);
        });
    }
    public static void MapDelete(this IEndpointRouteBuilder endpoints, string route, Type requestType, Type responseType)
    {
        endpoints.MapDelete(route, async (IMediator mediator, HttpContext context) =>
        {
            var request = Activator.CreateInstance(requestType);
            var result = await mediator.Send(request!);
            await context.Response.WriteAsJsonAsync(result);
        });
    }
    public static void MapPatch(this IEndpointRouteBuilder endpoints, string route, Type requestType, Type responseType)
    {
        endpoints.MapPatch(route, async (IMediator mediator, HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync(requestType);
            var result = await mediator.Send(request!);
            await context.Response.WriteAsJsonAsync(result);
        });
    }
    private static object CreateAndPopulateRequest(Type requestType, IQueryCollection query)
    {
        var request = Activator.CreateInstance(requestType);
        PopulateRequestFromQuery(request!, query);
        return request!;
    }
    private static void PopulateRequestFromQuery(object request, IQueryCollection query)
    {
        var requestType = request.GetType();

        foreach (var property in requestType.GetProperties())
        {
            if (query.ContainsKey(property.Name))
            {
                var value = query[property.Name].ToString();
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(request, convertedValue);
            }
        }
    }
}
