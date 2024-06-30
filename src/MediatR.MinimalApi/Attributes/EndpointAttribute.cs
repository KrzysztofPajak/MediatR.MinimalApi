namespace MediatR.MinimalApi.Attributes;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EndpointAttribute : Attribute
{
    public string Route { get; }
    public HttpMethod Method { get; }
    public string TagName { get; }

    public EndpointAttribute(string route, HttpMethod method, string tagName = "")
    {
        Route = route;
        Method = method;
        TagName = tagName;
    }
}