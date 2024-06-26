using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace MediatR.MinimalApi.Tests.Integration
{
    public class UserIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UserIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateUser_ShouldReturnSuccess()
        {
            //Arrange
            var client = _factory.CreateClient();
            var createUserModel = new
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Email = "test@example.com"
            };

            //Act
            var response = await client.PostAsJsonAsync("/user/create", createUserModel);

            //Assert            
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreateUser_InValidData_ShouldReturnBadRequest()
        {
            //Arrange
            var client = _factory.CreateClient();
            var createUserModel = new
            {
                FirstName = "",
                LastName = "TestLastName",
                Email = "test@example.com"
            };

            //Act
            var response = await client.PostAsJsonAsync("/user/create", createUserModel);

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnSuccess()
        {
            //Arrange
            var client = _factory.CreateClient();
            var updateUserModel = new
            {
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updated@example.com"
            };
            //Act
            var response = await client.PutAsJsonAsync($"/user/update/{Guid.NewGuid()}", updateUserModel);

            //Assert
            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task DeleteUser_ShouldReturnSuccess()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.DeleteAsync($"/user/delete/{Guid.NewGuid()}");

            //Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
