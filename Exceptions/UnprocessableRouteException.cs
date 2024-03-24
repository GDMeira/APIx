using System.Net;

namespace APIx.Exceptions;

public class UnprocessableRouteException(string message) : AppException(HttpStatusCode.UnprocessableEntity, message)
{
}