using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Models;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace MediatRSampleWebApplication.Queries.Users;

[Authorize]
[Endpoint("user/admin", HttpMethod.Get, "User")]
public record GetAuthorizeUsersQuery() : IRequest<IList<User>>;
