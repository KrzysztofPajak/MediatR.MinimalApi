namespace MediatR.MinimalApi.Exceptions;

public class HttpResponseException : Exception
{
    public int StatusCode { get; }

    public HttpResponseException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}