using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace WebApplication.Commands.Users;

[Endpoint("user/Delete/{Id}", HttpMethod.Delete, "User")]
public record DeleteUserCommand([FromRoute] Guid Id) : IRequest<bool>;
