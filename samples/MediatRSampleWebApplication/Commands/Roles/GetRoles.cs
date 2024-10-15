using MediatR;
using MediatRSampleWebApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace MediatRSampleWebApplication.Commands.Roles
{
    public static class GetRoles
    {
        [Authorize]
        public record GetRolesCommand : IRequest<IList<Role>>;

        public class GetRolesCommandHandler : IRequestHandler<GetRolesCommand, IList<Role>>
        {
            public Task<IList<Role>> Handle(GetRolesCommand request, CancellationToken cancellationToken)
            {
                var roles = new List<Role>
                {
                    new Role("Test1", Guid.NewGuid()),
                    new Role("Test2", Guid.NewGuid())
                };

                return Task.FromResult<IList<Role>>(roles);
            }
        }
    }
}
