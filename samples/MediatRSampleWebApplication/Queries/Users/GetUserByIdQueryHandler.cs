using MediatR;
using MediatR.MinimalApi.Exceptions;
using WebApplication.Models;

namespace MediatRSampleWebApplication.Queries.Users;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty) throw new HttpResponseException(400, "UserId cannot be empty.");

        return await Task.FromResult(new User("John", "Doe", "email@email.com") { Id = request.UserId });
    }
}