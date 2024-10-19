using MediatR.MinimalApi.Tests.Integration.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using WebApplication.Models;
using Xunit;

namespace MediatR.MinimalApi.Tests.Integration
{
    public class UserIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UserIntegrationTests(CustomWebApplicationFactory<Program> factory)
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

        [Fact]
        public async Task GetUserById_ShouldReturnUser()
        {
            //Arrange
            var client = _factory.CreateClient();
            var userId = Guid.NewGuid();
            //Act
            var response = await client.GetAsync($"/user/{userId}");

            //Assert
            response.EnsureSuccessStatusCode();
            var retrievedUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.Equal(userId, retrievedUser!.Id);
        }

        [Fact]
        public async Task GetUserById_EmptyGuid_ShouldReturnBadRequest()
        {
            //Arrange
            var client = _factory.CreateClient();
            var userId = Guid.Empty;
            //Act
            var response = await client.GetAsync($"/user/{userId}");

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorizeUsers_WithAutorization_ShouldReturnUser()
        {
            //Arrange
            var client = _factory.CreateClient();
            var token = JwtExtensions.GenerateJwtToken("TestIssuer", "TestAudience", "TestSuperSecretKeyThatIsAtLeast32CharactersLong");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await client.GetAsync($"/user/admin");

            //Assert
            response.EnsureSuccessStatusCode();
            var retrievedUser = await response.Content.ReadFromJsonAsync<List<User>>();
            Assert.Equal(2, retrievedUser!.Count);
        }

        [Fact]
        public async Task GetAuthorizeUsers_WithoutAutorization_ShouldReturnUnauthorized()
        {
            //Arrange
            var client = _factory.CreateClient();
            
            //Act
            var response = await client.GetAsync($"/user/admin");

            //Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
