namespace MediatR.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EndpointAttribute : Attribute
{
    public string Route { get; }
    public Models.HttpMethod Method { get; }

    public EndpointAttribute(string route, Models.HttpMethod method)
    {
        Route = route;
        Method = method;
    }
}