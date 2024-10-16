using MediatR.MinimalApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace MediatR.MinimalApi.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var hasAllowAnonymous = request.GetType().GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
        if (hasAllowAnonymous)
        {
            return await next();
        }

        var authorizeAttributes = request.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true).OfType<AuthorizeAttribute>();
        if (authorizeAttributes.Any())
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                throw new HttpResponseException(401, "User is not authenticated.");
            }

            foreach (var authorizeAttribute in authorizeAttributes.Where(x => x.Policy != null))
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(user, request, authorizeAttribute.Policy!);
                if (!authorizationResult.Succeeded)
                {
                    throw new HttpResponseException(401, "User is not authorized to perform this action.");
                }
            }
        }
        return await next();
    }
}

