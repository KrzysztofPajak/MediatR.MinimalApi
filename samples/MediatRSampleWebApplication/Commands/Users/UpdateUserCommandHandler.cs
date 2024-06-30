using MediatR;
using WebApplication.Models;

namespace WebApplication.Commands.Users;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(ILogger<CreateUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update user with first name {FirstName}, last name {LastName}, and email {Email} for Id {Id}", request.FirstName, request.LastName, request.Email, request.Id);

        var user = new User(request.FirstName, request.LastName, request.Email)
        {
            Id = request.Id,
        };

        return await Task.FromResult(user);
    }
}
