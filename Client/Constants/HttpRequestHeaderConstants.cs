namespace Fusion.Client.Constants;

internal static class HttpRequestHeaderConstants
{
    /// <summary>
    /// Denotes the HTTP header key utilized for specifying the unique identifier of a customer.
    /// </summary>
    public const string CustomerId = "CUSTOMER-ID";

    /// <summary>
    /// Represents the HTTP header key used to indicate the originating service of the request.
    /// </summary>
    public const string FromService = "SHAHR-FROM";

    /// <summary>
    /// Represents the HTTP header key used to specify the environment of the originating service.
    /// </summary>
    public const string FromServiceEnvironment = "SHAHR-FROM-ENVIRONMENT";
}