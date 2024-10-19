using MediatR.MinimalApi.Tests.Integration.Extensions;
using MediatRSampleWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.MinimalApi.Tests.Integration;
public class CompanyIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CompanyIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCompany_ShouldReturnUnauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();
        var createCompanyModel = new
        {
            Name = "TestRoleTestName"
        };

        //Act
        var response = await client.PostAsJsonAsync("/company/create", createCompanyModel);

        //Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCompany_Authorized_User_ShouldReturnBadRequest()
    {
        //Arrange
        var client = _factory.CreateClient();
        var createCompanyModel = new
        {
            Name = ""
        };
        var token = JwtExtensions.GenerateJwtToken("TestIssuer", "TestAudience", "TestSuperSecretKeyThatIsAtLeast32CharactersLong");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await client.PostAsJsonAsync("/company/create", createCompanyModel);

        //Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCompanies_ShouldReturnCompany_Without_Balance()
    {
        //Arrange
        var client = _factory.CreateClient();

        //Act
        var response = await client.GetAsync("/company/get");

        //Assert
        response.EnsureSuccessStatusCode();
        var role = await response.Content.ReadFromJsonAsync<List<Company>>();
        Assert.NotEmpty(role!);
        Assert.Equal(2, role!.Count);
        Assert.NotNull(role);
        Assert.Equal(0, role.First().Balance);
    }
}
