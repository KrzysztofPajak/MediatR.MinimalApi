using MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Reflection;

namespace MediatR.MinimalApi.Extensions
{
    internal static class OpenApiExtensions
    {
        public static void WithOpenApiDescription(this RouteHandlerBuilder builder, Type requestType)
        {
            var attribute = requestType.GetCustomAttribute<EndpointAttribute>();
            var responseType = GetResponseType(requestType);

            var operation = CreateOpenApiOperation(requestType, attribute!);
            operation.Parameters = ExtractParameters(requestType);

            if (!string.IsNullOrEmpty(attribute!.TagName))
            {
                operation.Tags = new List<OpenApiTag> { new() { Name = attribute.TagName } };
            }

            if (attribute.Method != Models.HttpMethod.GET)
            {
                operation.RequestBody = CreateRequestBody(requestType);
                builder.WithMetadata(new AcceptsMetadata([MediaTypeNames.Application.Json], requestType));
            }

            var producesResponseTypeAttributes = requestType.GetCustomAttributes<ProducesResponseTypeAttribute>().ToList();
            AddProducesResponseTypeMetadata(builder, responseType, producesResponseTypeAttributes);

            operation.Responses = CreateResponses(responseType);
            builder.WithMetadata(operation);
        }

        private static Type GetResponseType(Type requestType)
        {
            return requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];
        }

        private static OpenApiOperation CreateOpenApiOperation(Type requestType, EndpointAttribute attribute)
        {
            return new OpenApiOperation
            {
                Summary = $"Operation for {requestType.Name}",
                Description = $"Handles requests for {requestType.Name}"
            };
        }

        private static OpenApiParameter[] ExtractParameters(Type requestType)
        {
            var propertyParameters = GetPropertyParameters(requestType);
            var constructorParameters = GetConstructorParameters(requestType);

            return propertyParameters.Concat(constructorParameters).ToArray();
        }
        private static IEnumerable<OpenApiParameter> GetPropertyParameters(Type requestType)
        {
            return requestType.GetProperties()
                .Where(property => property.GetCustomAttribute<FromQueryAttribute>() is not null)
                .Select(property => CreateOpenApiParameter(property.Name, property.PropertyType, property.GetCustomAttributes()));
        }
        private static IEnumerable<OpenApiParameter> GetConstructorParameters(Type requestType)
        {
            var constructor = requestType.GetConstructors().FirstOrDefault();

            if (constructor == null)
            {
                return Enumerable.Empty<OpenApiParameter>();
            }

            return constructor.GetParameters()
                .Where(param => param.GetCustomAttribute<FromQueryAttribute>() is not null)
                .Select(param => CreateOpenApiParameter(param.Name!, param.ParameterType, param.GetCustomAttributes()));
        }
        private static OpenApiParameter CreateOpenApiParameter(string name, Type parameterType, IEnumerable<Attribute> attributes)
        {
            var description = attributes.OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;
            var isRequired = attributes.OfType<RequiredAttribute>().Any();

            return new OpenApiParameter
            {
                Name = name,
                Description = description,
                In = ParameterLocation.Query,
                Required = isRequired,
                Schema = new OpenApiSchema { Type = parameterType.Name.ToLower() }
            };
        }

        private static OpenApiRequestBody CreateRequestBody(Type requestType)
        {
            return new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        MediaTypeNames.Application.Json,
                        new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema { Type = requestType.Name }
                        }
                    }
                }
            };
        }

        private static void AddProducesResponseTypeMetadata(RouteHandlerBuilder builder, Type responseType, List<ProducesResponseTypeAttribute> attributes)
        {
            if (attributes.Any())
            {
                attributes.ForEach(producesResponseType =>
                {
                    builder.WithMetadata(new ProducesResponseTypeMetadata(producesResponseType.StatusCode, producesResponseType.Type));
                });
            }
            else
            {
                builder.WithMetadata(new ProducesResponseTypeMetadata(200, responseType));
            }
        }

        private static OpenApiResponses CreateResponses(Type responseType)
        {
            return new OpenApiResponses
            {
                {
                    ((int)HttpStatusCode.OK).ToString(),
                    new OpenApiResponse
                    {
                        Description = "Success",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                MediaTypeNames.Application.Json,
                                new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema { Type = responseType.Name }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
