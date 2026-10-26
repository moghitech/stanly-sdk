using System.Net;

namespace Fusion.Exceptions.BusinessExceptions;

/// <summary>
/// NotFound exception
/// </summary>
public abstract class NotFoundException : BaseHttpException
{
    /// <summary>
    /// Represents an exception that is thrown when a requested resource or data cannot be found.
    /// </summary>
    protected NotFoundException() : base(message: "Data not found!", status: HttpStatusCode.NotFound)
    {
    }

    /// <summary>
    /// Represents an abstract exception that is thrown when a requested resource or data cannot be found.
    /// </summary>
    protected NotFoundException(string message) : base(message: message, status: HttpStatusCode.NotFound)
    {
    }
}