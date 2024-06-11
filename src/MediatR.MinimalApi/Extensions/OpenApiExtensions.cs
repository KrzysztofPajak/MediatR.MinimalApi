using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace MediatR.MinimalApi.Extensions
{
    internal static class OpenApiExtensions
    {
        public static void WithOpenApiDescription(this RouteHandlerBuilder builder, Type requestType)
        {
            var operation = new OpenApiOperation();
            operation.Summary = $"Operation for {requestType.Name}";
            operation.Description = $"Handles requests for {requestType.Name}";
            operation.Parameters = requestType.GetProperties().Select(property =>
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
            }).ToArray();
            builder.WithMetadata(operation);

        }
    }
}
