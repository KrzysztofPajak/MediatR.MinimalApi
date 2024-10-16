using MediatR;
using MediatRSampleWebApplication.Models;

namespace MediatRSampleWebApplication.Commands.Roles;

public static class GetCompany
{
    public record Companies : IRequest<IList<Company>>;

    public class Handler : IRequestHandler<Companies, IList<Company>>
    {
        public Task<IList<Company>> Handle(Companies request, CancellationToken cancellationToken)
        {
            var roles = new List<Company>
                {
                    new() { Name = "Company 1", Balance = 1000 },
                    new() { Name = "Company 2", Balance = 2000 },
                };

            return Task.FromResult<IList<Company>>(roles);
        }
    }
}