using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace MediatR.MinimalApi.Extensions;

internal static class EndpointAuthorizationExtensions
{
    public static RouteHandlerBuilder AddAuthorization(this RouteHandlerBuilder builder, Type handlerType)
    {
        var authorizeAttributes = handlerType.GetCustomAttributes()
                .Where(attr => attr.GetType() == typeof(AuthorizeAttribute))
                .ToList();

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            if (authorizeAttribute is AuthorizeAttribute authorize)
            {
                builder.RequireAuthorization(authorize);
            }
        }

        return builder;
    }
}
