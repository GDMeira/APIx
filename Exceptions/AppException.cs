using System.Net;

namespace APIx.Exceptions;

public class AppException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode = statusCode;
}