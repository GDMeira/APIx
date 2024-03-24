using System.Net;

namespace APIx.Exceptions;

public class NotFoundException(string message) : AppException(HttpStatusCode.NotFound, message)
{
}