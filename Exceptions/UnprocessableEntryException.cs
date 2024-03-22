using System.Net;

namespace APIx.Exceptions;

public class UnprocessableEntryException(string message) : AppException(HttpStatusCode.UnprocessableContent, message)
{
}