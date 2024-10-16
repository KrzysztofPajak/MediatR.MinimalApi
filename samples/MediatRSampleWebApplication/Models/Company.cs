using MediatR.MinimalApi.Attributes;

namespace MediatRSampleWebApplication.Models;
public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    [RoleBasedAccess("Admin", "Manager")]
    public double Balance { get; set; }
}
