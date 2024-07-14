using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MediatRSampleWebApplication.Commands.Roles
{
    public static class DeleteRole
    {
        public record DeleteRoleCommand([FromRoute] Guid Id) : IRequest<bool>;

        public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
        {
            public Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }
        }
    }
}
