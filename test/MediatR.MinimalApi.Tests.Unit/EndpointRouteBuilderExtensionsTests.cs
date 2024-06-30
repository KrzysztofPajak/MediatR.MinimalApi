using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using MediatR.MinimalApi.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediatR.MinimalApi.Tests.Unit;

public class EndpointRouteBuilderExtensionsTests
{
    
    private readonly HttpClient _client;

    public EndpointRouteBuilderExtensionsTests()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<YourGetRequestHandler>());
            })
            .Configure(app =>
            {
                app.UseRouting();
                
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGetWithMediatR<YourGetRequest, YourResponse>("/example-get");
                    endpoints.MapGetWithMediatR<YourGetWithQueryParamRequest, YourResponse>("/example-getbyid");
                    endpoints.MapPostWithMediatR<YourPostRequest, YourResponse>("/example-post");
                    endpoints.MapPostWithMediatR<YourPostWithQueryParamRequest, YourResponse>("/example-post-param");
                    endpoints.MapDeleteWithMediatR<YourDeleteRequest, YourResponse>("/example-delete");
                    endpoints.MapPatchWithMediatR<YourPatchRequest, YourResponse>("/example-patch");
                    endpoints.MapPutWithMediatR<YourPutRequest, YourResponse>("/example-put");
                });
            });
        
        var server = new TestServer(builder);

        _client = server.CreateClient();
    }

    [Fact]
    public async Task Get_Should_Invoke_Mediator()
    {
        // Arrange
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/example-get");

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);

        // Assert
        httpResponse.EnsureSuccessStatusCode();
        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("Received", result.Result);
    }

    [Fact]
    public async Task Get_With_Id_Param_Should_Invoke_Mediator()
    {
        var param = 1;
        // Arrange
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/example-getbyid?id={param}");

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);

        // Assert
        httpResponse.EnsureSuccessStatusCode();
        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal($"Received: id-{param}", result.Result);
    }

    [Fact]
    public async Task Post_With_Body_Should_Invoke_Mediator()
    {
        // Arrange
        var request = new YourPostRequest.Test() { Test1 = "aaa", Test2 = "bbb" };
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/example-post")
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("Received: Test1:aaa, Test2:bbb", result.Result);
    }

    [Fact]
    public async Task Post_With_Body_And_QueryParam_Should_Invoke_Mediator()
    {
        // Arrange
        var request = new YourPostRequest.Test() { Test1 = "aaa", Test2 = "bbb" };
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/example-post-param?Id=1")
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("Received: Id:1 Test1:aaa, Test2:bbb", result.Result);
    }
    [Fact]
    public async Task Patch_Should_Invoke_Mediator()
    {
        // Arrange
        var request = new YourPatchRequest.Test() { Test1 = "aaa", Test2 = "bbb" };
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/example-patch?Id=1")
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("Patch: Id:1 Test1:aaa, Test2:bbb", result.Result);
    }
    [Fact]
    public async Task Put_Should_Invoke_Mediator()
    {
        // Arrange
        var request = new YourPatchRequest.Test() { Test1 = "aaa", Test2 = "bbb" };
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/example-put?Id=1")
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);
        httpResponse.EnsureSuccessStatusCode();

        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("Put: Id:1 Test1:aaa, Test2:bbb", result.Result);
    }
    [Fact]
    public async Task Delete_Should_Invoke_Mediator()
    {
        var param = 1;
        // Arrange
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/example-delete?id={param}");

        // Act
        var httpResponse = await _client.SendAsync(requestMessage);

        // Assert
        httpResponse.EnsureSuccessStatusCode();
        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YourResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal($"Delete: Id:{param}", result.Result);
    }

    public class YourGetRequest() : IRequest<YourResponse>
    {

    }

    public class YourGetWithQueryParamRequest() : IRequest<YourResponse>
    {
        [FromQuery]
        public int Id { get; set; }

    }
    public class YourPostRequest : IRequest<YourResponse>
    {
        [FromBody]
        public Test? Data { get; set; }

        public class Test
        {
            public string? Test1 { get; set; }
            public string? Test2 { get; set; }
        }
    }

    public class YourPostWithQueryParamRequest : IRequest<YourResponse>
    {
        [FromBody]
        public Test? Data { get; set; }
        public int Id { get; set; }

        public class Test
        {
            public string? Test1 { get; set; }
            public string? Test2 { get; set; }
        }
    }

    public class YourDeleteRequest : IRequest<YourResponse>
    {
        [FromQuery]
        public int Id { get; set; }

    }

    public class YourPatchRequest : IRequest<YourResponse>
    {
        [FromBody]
        public Test? Data { get; set; }
        public int Id { get; set; }

        public class Test
        {
            public string? Test1 { get; set; }
            public string? Test2 { get; set; }
        }
    }

    public class YourPutRequest : IRequest<YourResponse>
    {
        [FromBody]
        public Test? Data { get; set; }
        public int Id { get; set; }

        public class Test
        {
            public string? Test1 { get; set; }
            public string? Test2 { get; set; }
        }
    }
    public class YourResponse
    {
        public string? Result { get; set; }
    }

    public class YourGetRequestHandler : IRequestHandler<YourGetRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourGetRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Received" });
        }
    }
    public class YourGetWithQueryParamRequestHandler : IRequestHandler<YourGetWithQueryParamRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourGetWithQueryParamRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Received: id-{request.Id}" });
        }
    }
    public class YourPostRequestHandler : IRequestHandler<YourPostRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourPostRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Received: Test1:{request.Data!.Test1}, Test2:{request.Data.Test2}" });
        }
    }
    public class YourPostWithQueryParamRequestHandler : IRequestHandler<YourPostWithQueryParamRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourPostWithQueryParamRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Received: Id:{request.Id} Test1:{request.Data!.Test1}, Test2:{request.Data.Test2}" });
        }
    }
    public class YourDeleteRequestHandler : IRequestHandler<YourDeleteRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourDeleteRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Delete: Id:{request.Id}"});
        }
    }
    public class YourPatchRequestHandler : IRequestHandler<YourPatchRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourPatchRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Patch: Id:{request.Id} Test1:{request.Data!.Test1}, Test2:{request.Data.Test2}" });
        }
    }
    public class YourPutRequestHandler : IRequestHandler<YourPutRequest, YourResponse>
    {
        public async Task<YourResponse> Handle(YourPutRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new YourResponse { Result = $"Put: Id:{request.Id} Test1:{request.Data!.Test1}, Test2:{request.Data.Test2}" });
        }
    }
}
