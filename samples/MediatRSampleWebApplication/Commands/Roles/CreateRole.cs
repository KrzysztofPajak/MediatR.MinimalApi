using MediatR;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediatRSampleWebApplication.Commands.Roles
{
    public static class CreateRole
    {
        public record CreateRoleRequest(string Name);

        public record CreateRoleCommand([FromBody] CreateRoleRequest Role) : IRequest<Role>;

        public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Role>
        {
            public Task<Role> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                var role = new Role(request.Role.Name, Guid.NewGuid());
                return Task.FromResult(role);
            }
        }
    }
}
