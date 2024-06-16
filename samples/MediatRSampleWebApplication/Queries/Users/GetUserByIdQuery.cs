using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace MediatRSampleWebApplication.Queries.Users;

[Endpoint("user/{id}", HttpMethod.Get, "User")]
public record GetUserByIdQuery([FromQuery] Guid UserId) : IRequest<User>;
