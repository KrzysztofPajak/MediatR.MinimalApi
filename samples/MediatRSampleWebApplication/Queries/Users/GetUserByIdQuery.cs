using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace MediatRSampleWebApplication.Queries.Users;

[Endpoint("user/{UserId}", HttpMethod.Get, "User")]
public record GetUserByIdQuery([FromRoute] Guid UserId) : IRequest<User>;
