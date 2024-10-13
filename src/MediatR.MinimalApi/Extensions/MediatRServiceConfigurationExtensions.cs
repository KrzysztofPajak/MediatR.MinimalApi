using MediatR.MinimalApi.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.MinimalApi.Extensions;

public static class MediatRServiceConfigurationExtensions
{
    public static MediatRServiceConfiguration AddMinimalApi(this MediatRServiceConfiguration cfg, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        AddMinimalApiAuthorization(cfg, serviceLifetime);
        AddMinimalApiRoleBased(cfg, serviceLifetime);
        AddMinimalApiValidation(cfg, serviceLifetime);
        return cfg;
    }

    public static MediatRServiceConfiguration AddMinimalApiRoleBased(this MediatRServiceConfiguration cfg, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        cfg.AddOpenRequestPostProcessor(typeof(RoleBasedPostProcessor<,>));
        return cfg;
    }

    public static MediatRServiceConfiguration AddMinimalApiValidation(this MediatRServiceConfiguration cfg, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        return cfg;
    }
    public static MediatRServiceConfiguration AddMinimalApiAuthorization(this MediatRServiceConfiguration cfg, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        return cfg;
    }
}
