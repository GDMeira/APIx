using System.Net;

namespace APIx.Exceptions;

public class UnauthorizedException(string message) : AppException(HttpStatusCode.Unauthorized, message)
{
}