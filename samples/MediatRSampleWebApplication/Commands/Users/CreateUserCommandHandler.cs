using MediatR;
using WebApplication.Models;

namespace WebApplication.Commands.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with first name {FirstName}, last name {LastName}, and email {Email}", request.FirstName, request.LastName, request.Email);

        var user = new User(request.FirstName, request.LastName, request.Email);

        return await Task.FromResult(user);
    }
}
