using MediatR;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediatRSampleWebApplication.Commands.Roles
{
    public static class UpdateRole
    {
        public record UpdateRoleRequest(string Name);

        public record UpdateRoleCommand([FromRoute] Guid Id, [FromBody] UpdateRoleRequest Role) : IRequest<Role>;

        public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Role>
        {
            public Task<Role> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
            {
                var role = new Role(request.Role.Name, request.Id);
                return Task.FromResult(role);
            }
        }
    }
}
