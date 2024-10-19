namespace MediatR.MinimalApi.Exceptions;

public class HttpResponseException : Exception
{
    public int StatusCode { get; }

    public Dictionary<string, object?>? Extensions { get; }

    public HttpResponseException(int statusCode, string message, Dictionary<string, object?>? extensions = null) : base(message)
    {
        StatusCode = statusCode;
        Extensions = extensions;
    }
}
