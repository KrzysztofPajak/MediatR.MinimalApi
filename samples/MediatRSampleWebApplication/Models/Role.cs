using MediatR.MinimalApi.Attributes;

namespace MediatRSampleWebApplication.Models;
public record Role([property: RoleBasedAccess("Admin", "Manager")] string Name, Guid Id);
