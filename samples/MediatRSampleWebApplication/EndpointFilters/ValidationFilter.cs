using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MediatRSampleWebApplication.EndpointFilters
{
    public class ValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;
            var handlerType = httpContext.Items["HandlerType"] as Type;

            if (handlerType == null)
            {
                return Results.BadRequest("Handler type not found.");
            }

            var body = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
            var requestInstance = JsonSerializer.Deserialize(body, handlerType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(requestInstance!);

            if (!Validator.TryValidateObject(requestInstance!, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }

            context.Arguments[0] = requestInstance;

            return await next(context);
        }
    }
}
