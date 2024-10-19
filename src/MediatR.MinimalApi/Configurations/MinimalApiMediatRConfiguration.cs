namespace MediatR.MinimalApi.Configurations;

public class MinimalApiMediatRConfiguration
{
    public bool UseValidationBehavior { get; set; } = true;
    public bool UseAuthorizationBehavior { get; set; } = true;
    public bool UseRoleBasedPostProcessor { get; set; } = true;
}
