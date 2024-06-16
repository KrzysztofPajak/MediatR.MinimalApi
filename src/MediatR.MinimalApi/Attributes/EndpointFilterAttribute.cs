using Microsoft.AspNetCore.Http;

namespace MediatR.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EndpointFilterAttribute<T> : Attribute where T: IEndpointFilter
{
    public Type FilterType { get; }

    public EndpointFilterAttribute()
    {
        FilterType = typeof(T);
    }
}