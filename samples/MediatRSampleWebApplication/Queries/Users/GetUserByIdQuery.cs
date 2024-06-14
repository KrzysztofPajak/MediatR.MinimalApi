using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace MediatRSampleWebApplication.Queries.Users;

[Endpoint("user/{id}", MediatR.MinimalApi.Models.HttpMethod.GET, "User")]
public record GetUserByIdQuery([FromQuery] Guid UserId) : IRequest<User>;
