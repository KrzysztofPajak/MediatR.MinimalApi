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
            return requestType.GetProperties()
                .Where(property => property.GetCustomAttribute<FromQueryAttribute>() is not null)
                .Select(property =>
                {
                    var name = property.Name;
                    var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
                    var parameterType = property.PropertyType;

                    return new OpenApiParameter
                    {
                        Name = name,
                        Description = description,
                        In = ParameterLocation.Query,
                        Required = property.GetCustomAttribute<RequiredAttribute>() != null,
                        Schema = new OpenApiSchema { Type = parameterType.Name.ToLower() }
                    };
                })
                .ToArray();
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

        /*public static void WithOpenApiDescription(this RouteHandlerBuilder builder, Type requestType)
        {
            var attribute = requestType.GetCustomAttribute<EndpointAttribute>();

            var responseType = requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];

            var operation = new OpenApiOperation();
            operation.Summary = $"Operation for {requestType.Name}";
            operation.Description = $"Handles requests for {requestType.Name}";
            operation.Parameters = requestType.GetProperties().Select(property =>
            {
                if (property.GetCustomAttribute<FromQueryAttribute>() is not null)
                {
                    var name = property.Name;
                    var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
                    var parameterType = property.PropertyType;
                    return new OpenApiParameter
                    {
                        Name = name,
                        Description = description,
                        In = ParameterLocation.Query,
                        Required = property.GetCustomAttribute<RequiredAttribute>() != null,
                        Schema = new OpenApiSchema
                        {
                            Type = parameterType.Name.ToLower()
                        }
                    };
                }
                return null;
            }).Where(x => x is not null).ToArray();

            if (!string.IsNullOrEmpty(attribute!.TagName))
            {
                operation.Tags = new List<OpenApiTag> {
                    new OpenApiTag
                    {
                        Name = attribute.TagName
                    }
                };
            }
            if (attribute.Method != Models.HttpMethod.GET)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            MediaTypeNames.Application.Json,
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = requestType.Name
                                }
                            }
                        }
                    }
                };
                builder.WithMetadata(new AcceptsMetadata(["application/json"], requestType));
            }
            var producesResponseTypeAttribute = requestType.GetCustomAttributes<ProducesResponseTypeAttribute>();
            if (producesResponseTypeAttribute.Any())
            {
                producesResponseTypeAttribute.ToList().ForEach(producesResponseType =>
                {
                    builder.WithMetadata(new ProducesResponseTypeMetadata(producesResponseType.StatusCode, producesResponseType.Type));
                });
            }
            else
            {
                builder.WithMetadata(new ProducesResponseTypeMetadata(200, responseType));
            }
            operation.Responses = new()
            {
                {
                    ((int)HttpStatusCode.OK).ToString(),
                    new OpenApiResponse()
                   {
                        Description = "Success",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                MediaTypeNames.Application.Json,
                                new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {

                                        Type = responseType.Name,
                                    }
                                }
                            }
                        }
                    }
                }
            };
            

            builder.WithMetadata(operation);
        }*/
    }
}
