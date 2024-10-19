using FluentAssertions;
using FluentValidation;
using MediatR.MinimalApi.Behaviors;
using MediatR.MinimalApi.Exceptions;
using MediatR.MinimalApi.Tests.Unit.Fakes;
using Xunit;

namespace MediatR.MinimalApi.Tests.Unit.Behaviors
{
    // Unit Tests for ValidationBehavior
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var validators = new List<IValidator<ValidationRoleCommandFake>>
            {
                new ValidationRoleCommandValidatorFake()
            };
            var behavior = new ValidationBehavior<ValidationRoleCommandFake, MediatR.Unit>(validators);
            var command = new ValidationRoleCommandFake { Name = "" }; // Invalid data

            // Act
            Func<Task> act = async () => await behavior.Handle(command, () => Task.FromResult(MediatR.Unit.Value), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<HttpResponseException>();
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenValidationPasses()
        {
            // Arrange
            var validators = new List<IValidator<ValidationRoleCommandFake>>
            {
                new ValidationRoleCommandValidatorFake()
            };
            var behavior = new ValidationBehavior<ValidationRoleCommandFake, MediatR.Unit>(validators);
            var command = new ValidationRoleCommandFake { Name = "Name" };
            var nextCalled = false;

            // Act
            await behavior.Handle(command, () => { nextCalled = true; return Task.FromResult(MediatR.Unit.Value); }, CancellationToken.None);

            // Assert
            nextCalled.Should().BeTrue();
        }
    }
}
