using MediatR.MinimalApi.Behaviors;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.MinimalApi.Extensions;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection MinimalApiMediatRExtensions(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
        services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(RoleBasedPostProcessor<,>));

        services.AddHttpContextAccessor();
        return services;
    }
}
