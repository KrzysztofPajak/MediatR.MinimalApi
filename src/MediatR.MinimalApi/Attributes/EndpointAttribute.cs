namespace MediatR.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EndpointAttribute : Attribute
{
    public string Route { get; }
    public Models.HttpMethod Method { get; }
    public string TagName { get; }

    public EndpointAttribute(string route, Models.HttpMethod method, string tagName = "")
    {
        Route = route;
        Method = method;
        TagName = tagName;
    }
}