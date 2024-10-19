using FluentAssertions;
using MediatR.MinimalApi.Behaviors;
using MediatR.MinimalApi.Exceptions;
using MediatR.MinimalApi.Tests.Unit.Fakes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MediatR.MinimalApi.Tests.Unit.Behaviors;

// Unit Tests for AuthorizationBehavior for Different Commands
public class AuthorizationBehaviorAdditionalTests
{
    [Fact]
    public async Task Handle_ShouldCallNext_WhenAuthorizeAttributeIsPresent_AndAuthorizationSucceeds()
    {
        // Arrange
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success);

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuthType"))
        });

        var behavior = new AuthorizationBehavior<AuthorizeRoleCommandFake, MediatR.Unit>(authorizationService.Object, httpContextAccessor.Object);
        var command = new AuthorizeRoleCommandFake();
        var nextCalled = false;

        // Act
        await behavior.Handle(command, () => { nextCalled = true; return Task.FromResult(MediatR.Unit.Value); }, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenAuthorizeAttributeIsPresent_AndUserIsNotAuthenticated()
    {
        // Arrange
        var authorizationService = new Mock<IAuthorizationService>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

        var behavior = new AuthorizationBehavior<AuthorizeRoleCommandFake, MediatR.Unit>(authorizationService.Object, httpContextAccessor.Object);
        var command = new AuthorizeRoleCommandFake();

        // Act
        Func<Task> act = async () => await behavior.Handle(command, () => Task.FromResult(MediatR.Unit.Value), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpResponseException>().WithMessage("User is not authenticated.");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsAuthenticated_ButHasDifferentRole()
    {
        // Arrange
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Failed);

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser"), new Claim(ClaimTypes.Role, "DifferentRole") }, "TestAuthType"))
        });

        var behavior = new AuthorizationBehavior<AuthorizeRoleCommandFake, MediatR.Unit>(authorizationService.Object, httpContextAccessor.Object);
        var command = new AuthorizeRoleCommandFake();

        // Act
        Func<Task> act = async () => await behavior.Handle(command, () => Task.FromResult(MediatR.Unit.Value), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpResponseException>().WithMessage("User is not authorized to perform this action.");
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoAuthorizeAttributeIsPresent()
    {
        // Arrange
        var authorizationService = new Mock<IAuthorizationService>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuthType"))
        });

        var behavior = new AuthorizationBehavior<NotAuthorizeRoleCommandFake, MediatR.Unit>(authorizationService.Object, httpContextAccessor.Object);
        var command = new NotAuthorizeRoleCommandFake();
        var nextCalled = false;

        // Act
        await behavior.Handle(command, () => { nextCalled = true; return Task.FromResult(MediatR.Unit.Value); }, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenAllowAnonymousAttributeIsPresent()
    {
        // Arrange
        var authorizationService = new Mock<IAuthorizationService>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();

        var behavior = new AuthorizationBehavior<AllowAnonymousRoleCommandFake, MediatR.Unit>(authorizationService.Object, httpContextAccessor.Object);
        var command = new AllowAnonymousRoleCommandFake();
        var nextCalled = false;

        // Act
        await behavior.Handle(command, () => { nextCalled = true; return Task.FromResult(MediatR.Unit.Value); }, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }
}

