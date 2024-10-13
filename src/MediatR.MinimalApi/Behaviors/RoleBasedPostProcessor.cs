using MediatR.MinimalApi.Attributes;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Reflection;

namespace MediatR.MinimalApi.Behaviors;

public class RoleBasedPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleBasedPostProcessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (response != null)
        {
            var userRoles = _httpContextAccessor.HttpContext?.User.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(r => r.Value).ToList();
            FilterPropertiesRecursively(response, userRoles);
        }
        return Task.CompletedTask;
    }

    private void FilterPropertiesRecursively(object response, List<string>? userRoles)
    {
        if (response == null) return;

        if (response is IEnumerable collection && response.GetType() != typeof(string))
        {
            foreach (var item in collection)
            {
                FilterPropertiesRecursively(item, userRoles);
            }
            return;
        }

        var properties = response.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var roleAttribute = property.GetCustomAttribute<RoleBasedAccessAttribute>();

            if (roleAttribute != null && (userRoles == null || !userRoles.Intersect(roleAttribute.Roles).Any()))
            {
                property.SetValue(response, null);
            }
            else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                var propertyValue = property.GetValue(response);
                if (propertyValue != null)
                {
                    FilterPropertiesRecursively(propertyValue, userRoles);
                }
            }
        }
    }
}