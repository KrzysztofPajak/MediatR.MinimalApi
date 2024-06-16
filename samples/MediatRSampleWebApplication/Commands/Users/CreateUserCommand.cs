using MediatR;
using MediatR.MinimalApi.Attributes;
using MediatRSampleWebApplication.EndpointFilters;
using System.ComponentModel.DataAnnotations;
using WebApplication.Models;

namespace WebApplication.Commands.Users;

[EndpointFilter<ValidationFilter>()]
[Endpoint("user/create", MediatR.MinimalApi.Models.HttpMethod.POST, "User")]
public class CreateUserCommand : IRequest<User>
{
    [MinLength(10)]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public CreateUserCommand(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }   
}