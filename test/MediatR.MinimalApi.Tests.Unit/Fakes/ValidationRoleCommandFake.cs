namespace MediatR.MinimalApi.Tests.Unit.Fakes;

public class ValidationRoleCommandFake :  IRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleCommandhandlerFake : IRequestHandler<ValidationRoleCommandFake>
{
    public Task Handle(ValidationRoleCommandFake request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
