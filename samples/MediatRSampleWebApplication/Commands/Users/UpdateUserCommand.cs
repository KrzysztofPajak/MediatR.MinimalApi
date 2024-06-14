using MediatR;
using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using WebApplication.Models;

namespace WebApplication.Commands.Users;

[Endpoint("user/update/{id}", MediatR.MinimalApi.Models.HttpMethod.POST, "User")]
public record UpdateUserCommand(string FirstName, string LastName, string Email) : IRequest<User>
{
    [JsonIgnore]
    [FromQuery]
    public Guid Id { get; set; }
}