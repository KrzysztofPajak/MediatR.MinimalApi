using MediatR.MinimalApi.Attributes;
using MediatR.MinimalApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Shouldly;
using System.Reflection;
using Xunit;

namespace MediatR.MinimalApi.Tests.Unit
{
    public class OpenApiExtensionsTests
    {
        private readonly Type _requestType;
        private readonly Type _requestTypePost;

        public OpenApiExtensionsTests()
        {
            _requestType = typeof(SampleRequest);
            _requestTypePost = typeof(SamplePostRequest);
        }

        [Fact]
        public void WithOpenApiDescription_AddsOperationMetadata()
        {
            void SpecificAddOpenApiDescription(RouteHandlerBuilder builder) => builder.WithOpenApiDescription(_requestType);

            void AssertMetadata(EndpointBuilder builder)
            {
                var metadata = builder.Metadata;
                Assert.Contains(metadata, m => m is OpenApiOperation);
            }
            RunWithBothBuilders(SpecificAddOpenApiDescription, AssertMetadata);
        }

        [Fact]
        public void GetResponseType_ReturnsCorrectType()
        {
            void SpecificAddOpenApiDescription(RouteHandlerBuilder builder) => builder.WithOpenApiDescription(_requestType);

            void AssertMetadata(EndpointBuilder builder)
            {
                var metadata = builder.Metadata;
                var openApiOperation = (OpenApiOperation)metadata.FirstOrDefault(m => m is OpenApiOperation)!;
                openApiOperation.Responses.Count.ShouldBe(1);
                var responseType = openApiOperation.Responses.FirstOrDefault().Value.Content.FirstOrDefault().Value.Schema.Type;
                Assert.Equal(typeof(SampleResponse).Name, responseType);
            }
            RunWithBothBuilders(SpecificAddOpenApiDescription, AssertMetadata);

        }

        [Fact]
        public void Parameters_ReturnsCorrectParameters()
        {
            void SpecificAddOpenApiDescription(RouteHandlerBuilder builder) => builder.WithOpenApiDescription(_requestType);

            void AssertMetadata(EndpointBuilder builder)
            {
                var metadata = builder.Metadata;
                var openApiOperation = (OpenApiOperation)metadata.FirstOrDefault(m => m is OpenApiOperation)!;
                openApiOperation.RequestBody.ShouldBeNull();
                openApiOperation.Responses.Count.ShouldBe(1);
                openApiOperation.Parameters.Count.ShouldBe(1);
                var param = openApiOperation.Parameters.FirstOrDefault()!.Name;
                Assert.Equal("QueryParam", param);
            }
            RunWithBothBuilders(SpecificAddOpenApiDescription, AssertMetadata);
        }

        [Fact]
        public void CreateRequestBody_ReturnsCorrectRequestBody()
        {

            void SpecificAddOpenApiDescription(RouteHandlerBuilder builder) => builder.WithOpenApiDescription(_requestTypePost);

            void AssertMetadata(EndpointBuilder builder)
            {
                var metadata = builder.Metadata;
                var openApiOperation = (OpenApiOperation)metadata.FirstOrDefault(m => m is OpenApiOperation)!;
                openApiOperation.RequestBody.ShouldNotBeNull();
            }
            RunWithBothBuilders(SpecificAddOpenApiDescription, AssertMetadata);

        }

        [Fact]
        public void AddProducesResponseTypeMetadata_AddsMetadataCorrectly()
        {
            void SpecificAddOpenApiDescription(RouteHandlerBuilder builder) => builder.WithOpenApiDescription(_requestTypePost);
            void AssertMetadata(EndpointBuilder builder)
            {
                var metadata = builder.Metadata;
                Assert.Contains(metadata, m => m is ProducesResponseTypeMetadata);
            }
            
            RunWithBothBuilders(SpecificAddOpenApiDescription, AssertMetadata);
        }

        private void RunWithBothBuilders(Action<RouteHandlerBuilder> specificSetup, Action<EndpointBuilder> assert)
        {
            
            var routeTestBuilder = new TestEndointConventionBuilder();
            var routeHandlerBuilder = new RouteHandlerBuilder(new[] { routeTestBuilder });
            specificSetup(routeHandlerBuilder);
            assert(routeTestBuilder);
        }

        private sealed class TestEndointConventionBuilder : EndpointBuilder, IEndpointConventionBuilder
        {
            public void Add(Action<EndpointBuilder> convention)
            {
                convention(this);
            }

            public override Endpoint Build() => throw new NotImplementedException();
        }

        private T InvokePrivateMethod<T>(string methodName, params object[] parameters)
        {
            var method = typeof(OpenApiExtensions).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            return (T)method!.Invoke(null, parameters)!;
        }

        private void InvokePrivateMethod(string methodName, params object[] parameters)
        {
            var method = typeof(OpenApiExtensions).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            method!.Invoke(null, parameters);
        }

        [Endpoint("test", Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod.Get, "tag")]
        private class SampleRequest : IRequest<SampleResponse>
        {
            [FromQuery]
            public string? QueryParam { get; set; }
            public string? Id { get; set; }
        }

        [ProducesResponseType(200)]
        [Endpoint("test2", Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod.Post, "tag")]
        private class SamplePostRequest : IRequest<SampleResponse>
        {
            public string? Id { get; set; }
        }

        private class SampleResponse
        {
        }        

       
    }
}
