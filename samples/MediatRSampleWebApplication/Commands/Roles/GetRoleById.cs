using MediatR;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediatRSampleWebApplication.Commands.Roles
{
    public static class GetRoleById
    {
        public record GetRoleByIdCommand([FromRoute] Guid Id) : IRequest<Role>;

        public class GetRoleByIdCommandHandler : IRequestHandler<GetRoleByIdCommand, Role>
        {
            public Task<Role> Handle(GetRoleByIdCommand request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Role("Test", request.Id));
            }
        }
    }
}
