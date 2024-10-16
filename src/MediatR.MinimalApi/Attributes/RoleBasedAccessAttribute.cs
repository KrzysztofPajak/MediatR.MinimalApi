namespace MediatR.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RoleBasedAccessAttribute : Attribute
{
    public string[] Roles { get; }

    public RoleBasedAccessAttribute(params string[] roles)
    {
        Roles = roles;
    }
}
