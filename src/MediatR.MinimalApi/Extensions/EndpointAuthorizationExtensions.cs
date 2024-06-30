using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace MediatR.MinimalApi.Extensions;

internal static class EndpointAuthorizationExtensions
{
    internal static RouteHandlerBuilder AddAuthorization(this RouteHandlerBuilder builder, Type handlerType)
    {
        var authorizeAttributes = handlerType.GetCustomAttributes()
                .Where(attr => attr.GetType() == typeof(AuthorizeAttribute) || attr.GetType() == typeof(AllowAnonymousAttribute))
                .ToList();

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            switch (authorizeAttribute)
            {
                case AuthorizeAttribute authorize:
                    builder.RequireAuthorization(authorize);
                    break;
                case AllowAnonymousAttribute:
                    builder.AllowAnonymous();
                    break;
            }
        }

        return builder;
    }
}
