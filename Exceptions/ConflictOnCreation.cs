using System.Net;

namespace APIx.Exceptions;

public class ConflictOnCreation(string message) : AppException(HttpStatusCode.Conflict, message)
{
}