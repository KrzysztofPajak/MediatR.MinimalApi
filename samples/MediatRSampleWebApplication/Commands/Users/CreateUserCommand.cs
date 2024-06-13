using MediatR;
using MediatR.MinimalApi.Attributes;
using WebApplication.Models;

namespace WebApplication.Commands.Users;

[Endpoint("user/create", MediatR.MinimalApi.Models.HttpMethod.POST, "User")]
public class CreateUserCommand : IRequest<User>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public CreateUserCommand(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}
