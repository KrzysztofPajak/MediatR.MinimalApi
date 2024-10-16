using System.Net.Http.Json;
using Xunit;

namespace MediatR.MinimalApi.Tests.Integration;

public class RoleIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public RoleIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateRole_ShouldReturnSuccess()
    {
        //Arrange
        var client = _factory.CreateClient();
        var createRoleModel = new
        {
            Name = "TestRoleName"
        };

        //Act
        var response = await client.PostAsJsonAsync("/role/create", createRoleModel);

        //Assert            
        response.EnsureSuccessStatusCode();
        var role = await response.Content.ReadFromJsonAsync<Role>();
        Assert.NotNull(role);
        Assert.Equal("TestRoleName", role.Name);
    }

    [Fact]
    public async Task GetRoleById_ShouldReturnRole()
    {
        //Arrange
        var client = _factory.CreateClient();

        //Act
        var response = await client.GetAsync($"/role/get/{Guid.NewGuid()}");

        //Assert
        response.EnsureSuccessStatusCode();
        var role = await response.Content.ReadFromJsonAsync<Role>();
        Assert.NotNull(role);
    }

    [Fact]
    public async Task GetRoles_ShouldReturnRoles()
    {
        //Arrange
        var client = _factory.CreateClient();

        //Act
        var response = await client.GetAsync("/role/get");

        //Assert
        response.EnsureSuccessStatusCode();
        var role = await response.Content.ReadFromJsonAsync<List<Role>>();
        Assert.NotEmpty(role!);
        Assert.Equal(2, role!.Count);
        Assert.NotNull(role);
    }

    [Fact]
    public async Task UpdateRole_ShouldReturnSuccess()
    {
        //Arrange
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();
        var updateRoleModel = new 
        {
            Name = "TestRoleName"
        };

        //Act
        var response = await client.PutAsJsonAsync($"/role/update/{id}", updateRoleModel);

        //Assert            
        response.EnsureSuccessStatusCode();
        var role = await response.Content.ReadFromJsonAsync<Role>();
        Assert.NotNull(role);
        Assert.Equal("TestRoleName", role.Name);
        Assert.Equal(id, role.Id);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnSuccess()
    {
        //Arrange
        var client = _factory.CreateClient();
        var createRoleModel = new
        {
            Name = "TestRoleName"
        };

        //Act
        var response = await client.DeleteAsync($"/role/delete/{Guid.NewGuid()}");

        //Assert            
        response.EnsureSuccessStatusCode();
    }

    public class Test
    {
        public string? Name { get; set; }
    }
    record Role(string Name, Guid Id);
}
