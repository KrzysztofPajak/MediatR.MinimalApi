using MediatR.MinimalApi.Attributes;

namespace MediatR.MinimalApi.Tests.Unit.Fakes;

public class UserDataFake
{
    public string? Name { get; set; }

    [RoleBasedAccess("Admin")]
    public string? SensitiveData { get; set; }

    [RoleBasedAccess("Admin", "Manager")]
    public string? SemiSensitiveData { get; set; }
}

public class GetUserDataQueryFake : IRequest<UserDataFake>
{
}

public class GetUserDataHandlerFake : IRequestHandler<GetUserDataQueryFake, UserDataFake>
{
    public Task<UserDataFake> Handle(GetUserDataQueryFake request, CancellationToken cancellationToken)
    {
        var userData = new UserDataFake
        {
            Name = "John Doe",
            SensitiveData = "Secret Info",
            SemiSensitiveData = "Manager Info"
        };

        return Task.FromResult(userData);
    }
}

public class GetUserDataListQueryFake : IRequest<List<UserDataFake>>
{
}

public class GetUserDataListHandlerFake : IRequestHandler<GetUserDataListQueryFake, List<UserDataFake>>
{
    public Task<List<UserDataFake>> Handle(GetUserDataListQueryFake request, CancellationToken cancellationToken)
    {
        var userDataList = new List<UserDataFake>
        {
            new UserDataFake { Name = "John Doe", SensitiveData = "Secret Info", SemiSensitiveData = "Manager Info" },
            new UserDataFake { Name = "Jane Doe", SensitiveData = "Secret Info 2", SemiSensitiveData = "Manager Info 2" }
        };
        return Task.FromResult(userDataList);
    }
}
