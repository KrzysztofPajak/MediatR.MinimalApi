﻿using MediatR.MinimalApi.Behaviors;
using MediatR.MinimalApi.Configurations;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.MinimalApi.Extensions;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection MinimalApiMediatRExtensions(this IServiceCollection services, Action<MinimalApiMediatRConfiguration>? configure = null)
    {
        var configuration = new MinimalApiMediatRConfiguration();
        configure?.Invoke(configuration);

        var registrationActions = new List<Action>
        {
            () => { if (configuration.UseAuthorizationBehavior) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>)); },
            () => { if (configuration.UseValidationBehavior) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); },
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
