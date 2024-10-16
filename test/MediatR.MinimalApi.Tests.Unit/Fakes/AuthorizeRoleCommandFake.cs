using Microsoft.AspNetCore.Authorization;

namespace MediatR.MinimalApi.Tests.Unit.Fakes;

[Authorize("TestAuthType")]
public class AuthorizeRoleCommandFake :  IRequest
{
    public string Name { get; set; } = string.Empty;
}

public class NotAuthorizeRoleCommandFake : IRequest
{
    public string Name { get; set; } = string.Empty;
}

[AllowAnonymous]
public class AllowAnonymousRoleCommandFake : IRequest
{
    public string Name { get; set; } = string.Empty;
}