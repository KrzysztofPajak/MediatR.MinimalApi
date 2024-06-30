using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace MediatR.MinimalApi.Extensions;

internal static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder AddFiltersFromAttributes(this RouteHandlerBuilder builder, IServiceProvider serviceProvider, Type handlerType)
    {
        var filterAttributes = handlerType.GetCustomAttributes()
                .Where(attr => attr.GetType().IsGenericType && attr.GetType().GetGenericTypeDefinition() == typeof(EndpointFilterAttribute<>))
                .ToList();

        foreach (var filterAttribute in filterAttributes)
        {
            var filterType = filterAttribute.GetType().GetProperty("FilterType")?.GetValue(filterAttribute) as Type;
            var filterInstance = serviceProvider.GetService(filterType!) as IEndpointFilter;
            if (filterInstance != null)
            {
                builder.AddEndpointFilter(async (context, next) =>
                {
                    context.HttpContext.Items["HandlerType"] = handlerType;
                    return await filterInstance.InvokeAsync(context, next);
                });
            }
        }

        return builder;
    }
}
