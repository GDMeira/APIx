using System.Net;

namespace APIx.Exceptions;

public class ForbiddenOperationException(string message) : AppException(HttpStatusCode.Forbidden, message)
{
}