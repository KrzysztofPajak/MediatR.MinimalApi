using MediatR.MinimalApi.Attributes;
using MediatR.MinimalApi.Exceptions;
using MediatR.MinimalApi.Extensions;
using MediatR.MinimalApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace MediatR.MinimalApi.Endpoints
{
    internal static class MediatREndpoints
    {
        internal static void MediatREndpoint(this IEndpointRouteBuilder endpoints, IEnumerable<Type> handlerTypes)
        {
            var methodMappings = new Dictionary<HttpMethod, Func<string, Delegate, RouteHandlerBuilder>>
            {
                { HttpMethod.Get, endpoints.MapGet },
                { HttpMethod.Post, endpoints.MapPost },
                { HttpMethod.Put, endpoints.MapPut },
                { HttpMethod.Delete, endpoints.MapDelete },
                { HttpMethod.Patch, endpoints.MapPatch }
            };

            foreach (var handlerType in handlerTypes)
            {
                var attribute = handlerType.GetCustomAttribute<EndpointAttribute>();
                if (attribute == null) continue;

                var route = attribute.Route;
                var httpMethod = attribute.Method;
                if (!methodMappings.TryGetValue(httpMethod, out var mapMethod))
                {
                    throw new NotSupportedException($"Http method {httpMethod} is not supported.");
                }

                var responseType = handlerType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                    ?.GetGenericArguments()[0];
                if (responseType == null) continue;

                var routeHandlerBuilder = mapMethod(route, (Delegate)CreateRequestDelegate(handlerType, httpMethod));
                ConfigureEndpoint(routeHandlerBuilder, endpoints.ServiceProvider, handlerType);
            }
        }

        private static void ConfigureEndpoint(RouteHandlerBuilder endpointBuilder, IServiceProvider serviceProvider, Type handlerType)
        {
            endpointBuilder                
                .AddFiltersFromAttributes(serviceProvider, handlerType)
                .AddAuthorization(handlerType)
                .WithOpenApiDescription(handlerType);
        }

        private static RequestDelegate CreateRequestDelegate(Type requestType, HttpMethod httpMethod)
        {
            return async context =>
            {
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var request = await EndpointsHelpers.CreateRequestAsync(requestType, httpMethod, context.Request);

                try
                {
                    var result = await mediator.Send(request, context.RequestAborted);
                    await context.Response.WriteAsJsonAsync(result, context.RequestAborted);
                }
                catch (HttpResponseException ex)
                {
                    await EndpointsHelpers.HandleHttpResponseExceptionAsync(context, ex);
                }
                catch (Exception ex)
                {
                    await EndpointsHelpers.HandleExceptionAsync(context, ex);
                }
            };
        }
    }

}