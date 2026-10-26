using System.Net;

namespace Fusion.Exceptions;

public class BaseHttpException(
    string message,
    HttpStatusCode status,
    Dictionary<string, List<string>>? bulkMessages = null)
    : Exception(message)
{
    public int Status { get; private set; } = (int)status;
    public Dictionary<string, List<string>>? BulkMessages { get; private set; } = bulkMessages;
}
