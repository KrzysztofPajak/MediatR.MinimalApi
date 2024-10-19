using MediatR.MinimalApi.Behaviors;
using MediatR.MinimalApi.Configurations;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.MinimalApi.Extensions;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection MinimalApiMediatRExtensions(this IServiceCollection services, Action<MinimalApiMediatRConfiguration> configure)
    {
        var configuration = new MinimalApiMediatRConfiguration();
        configure(configuration);

        var registrationActions = new List<Action>
        {
            () => { if (configuration.UseValidationBehavior) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); },
            () => { if (configuration.UseAuthorizationBehavior) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>)); },
            () => { if (configuration.UseRoleBasedPostProcessor) {
                    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
                    services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(RoleBasedPostProcessor<,>)); }
                }
        };

        registrationActions.ForEach(action => action());

        services.AddHttpContextAccessor();
        return services;
    }
}
