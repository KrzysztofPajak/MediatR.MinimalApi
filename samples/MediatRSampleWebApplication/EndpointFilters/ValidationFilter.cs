using MediatR;
using System.ComponentModel.DataAnnotations;

using System.Text;
using System.Text.Json;

namespace MediatRSampleWebApplication.EndpointFilters
{
    public class ValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            var handlerType = httpContext.Items["HandlerType"] as Type;

            if (handlerType == null)
            {
                return Results.BadRequest("Handler type not found.");
            }

            request.EnableBuffering();

            var bodyAsString = await ReadRequestBodyAsync(request);

            if (!string.IsNullOrEmpty(bodyAsString))
            {
                var requestInstance = JsonSerializer.Deserialize(bodyAsString, handlerType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(requestInstance!);

                if (!Validator.TryValidateObject(requestInstance!, validationContext, validationResults, true))
                {                    
                    return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Errors", validationResults.Select(x => x.ErrorMessage).ToArray()! }
                    });
                }
            }
            else
                return Results.Problem(title: "Bad Request", detail: "Request body is empty.", statusCode: StatusCodes.Status400BadRequest);

            return await next(context);
        }
        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            return body;
        }

    }
}
