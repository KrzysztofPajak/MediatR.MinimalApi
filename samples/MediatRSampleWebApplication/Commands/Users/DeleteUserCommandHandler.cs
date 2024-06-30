using MediatR;

namespace WebApplication.Commands.Users;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete user with Id {Id}", request.Id);
        return await Task.FromResult(true);
    }
}
