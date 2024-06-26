using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using WebApplication.Models;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace WebApplication.Commands.Users;

[Endpoint("user/update/{Id}", HttpMethod.Put, "User")]
public record UpdateUserCommand(string FirstName, string LastName, string Email) : IRequest<User>
{
    [JsonIgnore]
    [FromRoute]
    public Guid Id { get; set; }
}