using FluentAssertions;
using MediatR.MinimalApi.Behaviors;
using MediatR.MinimalApi.Tests.Unit.Fakes;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MediatR.MinimalApi.Tests.Unit.Behaviors;
public class RoleBasedPostProcessorTests
{
    [Fact]
    public async Task Should_Filter_Out_SensitiveData_For_NonAdmin_User()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "User")
        }));

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var postProcessor = new RoleBasedPostProcessor<GetUserDataQueryFake, UserDataFake>(httpContextAccessor.Object);
        var userData = new UserDataFake
        {
            Name = "John Doe",
            SensitiveData = "Secret Info",
            SemiSensitiveData = "Manager Info"
        };

        // Act
        await postProcessor.Process(new GetUserDataQueryFake(), userData, CancellationToken.None);

        // Assert
        userData.SensitiveData.Should().BeNull();
        userData.SemiSensitiveData.Should().BeNull();
        userData.Name.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Not_Filter_Out_SensitiveData_For_Admin_User()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "Admin")
        }));
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var postProcessor = new RoleBasedPostProcessor<GetUserDataQueryFake, UserDataFake>(httpContextAccessor.Object);
        var userData = new UserDataFake
        {
            Name = "John Doe",
            SensitiveData = "Secret Info",
            SemiSensitiveData = "Manager Info"
        };

        // Act
        await postProcessor.Process(new GetUserDataQueryFake(), userData, CancellationToken.None);

        // Assert
        userData.SensitiveData.Should().NotBeNull();
        userData.SemiSensitiveData.Should().NotBeNull();
        userData.Name.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Filter_Out_SensitiveData_For_Manager_User()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "Manager")
        }));
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var postProcessor = new RoleBasedPostProcessor<GetUserDataQueryFake, UserDataFake>(httpContextAccessor.Object);
        var userData = new UserDataFake
        {
            Name = "John Doe",
            SensitiveData = "Secret Info",
            SemiSensitiveData = "Manager Info"
        };

        // Act
        await postProcessor.Process(new GetUserDataQueryFake(), userData, CancellationToken.None);

        // Assert
        userData.SensitiveData.Should().BeNull();
        userData.SemiSensitiveData.Should().NotBeNull();
        userData.Name.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Handle_Empty_Roles()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var postProcessor = new RoleBasedPostProcessor<GetUserDataQueryFake, UserDataFake>(httpContextAccessor.Object);
        var userData = new UserDataFake
        {
            Name = "John Doe",
            SensitiveData = "Secret Info",
            SemiSensitiveData = "Manager Info"
        };

        // Act
        await postProcessor.Process(new GetUserDataQueryFake(), userData, CancellationToken.None);

        // Assert
        userData.SensitiveData.Should().BeNull();
        userData.SemiSensitiveData.Should().BeNull();
        userData.Name.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Filter_List_Of_UserData_For_NonAdmin_User()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "User")
        }));
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var postProcessor = new RoleBasedPostProcessor<GetUserDataListQueryFake, List<UserDataFake>>(httpContextAccessor.Object);
        var userDataList = new List<UserDataFake>
        {
            new UserDataFake { Name = "John Doe", SensitiveData = "Secret Info", SemiSensitiveData = "Manager Info" },
            new UserDataFake { Name = "Jane Doe", SensitiveData = "Secret Info 2", SemiSensitiveData = "Manager Info 2" }
        };

        // Act
        await postProcessor.Process(new GetUserDataListQueryFake(), userDataList, CancellationToken.None);

        // Assert
        userDataList.Should().AllSatisfy(userData =>
        {
            userData.SensitiveData.Should().BeNull();
            userData.SemiSensitiveData.Should().BeNull();
            userData.Name.Should().NotBeNull();
        });
    }
}
