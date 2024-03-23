using System.Net;

namespace APIx.Exceptions;

public class ConflictOnCreationException(string message) : AppException(HttpStatusCode.Conflict, message)
{
}