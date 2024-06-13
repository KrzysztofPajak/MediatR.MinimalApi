using MediatR.MinimalApi.Attributes;
using MediatR.MinimalApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatR.MinimalApi.Endpoints
{
    internal static class MediatREndpoints
    {

        internal static void MediatREndpoint(this IEndpointRouteBuilder endpoints, IEnumerable<Type> handlerTypes)
        {
            foreach (var handler in handlerTypes)
            {
                var attribute = handler.GetCustomAttribute<EndpointAttribute>();
                if (attribute == null) continue;

                var route = attribute.Route;
                var httpMethod = attribute.Method;
                var responseType = handler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
                

                switch (httpMethod)
                {
                    case Models.HttpMethod.GET:
                        endpoints.MapGet(route, (Delegate)CreateRequestDelegate(handler, httpMethod))
                            .WithOpenApiDescription(handler);
                        break;
                    case Models.HttpMethod.POST:
                        endpoints.MapPost(route, (Delegate)CreateRequestDelegate(handler, httpMethod))                            
                            .WithOpenApiDescription(handler);
                        break;
                    case Models.HttpMethod.PUT:
                        endpoints.MapPut(route, (Delegate)CreateRequestDelegate(handler, httpMethod));
                        break;
                    case Models.HttpMethod.DELETE:
                        endpoints.MapDelete(route, (Delegate)CreateRequestDelegate(handler, httpMethod));
                        break;
                    case Models.HttpMethod.PATCH:
                        endpoints.MapPatch(route, (Delegate)CreateRequestDelegate(handler, httpMethod));
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

                var request = httpMethod switch
                {
                    Models.HttpMethod.GET or Models.HttpMethod.DELETE => CreateFromQueryRequest(requestType, context.Request),
                    Models.HttpMethod.POST or Models.HttpMethod.PUT or Models.HttpMethod.PATCH => await CreateFromBodyRequest(requestType, context.Request),
                    _ => throw new NotSupportedException($"Http method {httpMethod} is not supported.")
                };

                if (request == null) throw new InvalidOperationException("Request cannot be null.");

                var result = await mediator.Send(request);
                await context.Response.WriteAsJsonAsync(result);
            };
        }


        private static object? CreateFromQueryRequest(Type requestType, HttpRequest httpRequest)
        {            
            var request = Activator.CreateInstance(requestType);
            PopulateRequestFromQuery(request, httpRequest.Query);
            return request;
        }

        private static async ValueTask<object?> CreateFromBodyRequest(Type requestType, HttpRequest httpRequest)
        {
            return httpRequest.ContentLength is > 0 ? await httpRequest.ReadFromJsonAsync(requestType) : Activator.CreateInstance(requestType);
        }

        private static void PopulateRequestFromQuery(object? request, IQueryCollection query)
        {
            if (request == null) return;
            
            var requestType = request.GetType();
            foreach (var property in requestType.GetProperties())
            {
                if (!query.ContainsKey(property.Name)) continue;
                
                var value = query[property.Name].ToString();
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(request, convertedValue);
            }
        }
    }

}