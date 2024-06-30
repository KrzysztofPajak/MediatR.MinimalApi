using MediatR;
using WebApplication.Models;

namespace MediatRSampleWebApplication.Queries.Users;

public class GetAuthorizeUsersQueryHandler : IRequestHandler<GetAuthorizeUsersQuery, IList<User>>
{
    public async Task<IList<User>> Handle(GetAuthorizeUsersQuery request, CancellationToken cancellationToken)
    {

        return await Task.FromResult(new User[]
        {
            new User("Admin", "Doe", "admin1@email.com") 
            { 
                Id = Guid.NewGuid()
            },
            new User("Admin", "John", "admin2@email.com")
            {
                Id = Guid.NewGuid()
            }
        });
    }
}