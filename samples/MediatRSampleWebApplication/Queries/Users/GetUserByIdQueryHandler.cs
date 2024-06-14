using MediatR;
using WebApplication.Models;

namespace MediatRSampleWebApplication.Queries.Users;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new User("John", "Doe", "email@email.com") { Id = request.UserId });
    }
}