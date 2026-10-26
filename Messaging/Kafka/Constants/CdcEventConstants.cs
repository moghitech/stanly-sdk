namespace Fusion.Messaging.Kafka.Constants;

/// <summary>
/// Contains constant values used in CDC (Change Data Capture) event messaging within the Kafka integration module.
/// This class provides a centralized reference for event-related constants,
/// ensuring consistency and reducing the risk of hard-coded errors across the application.
/// </summary>
public static class CdcEventConstants
{
    /// <summary>
    /// Represents the header key used to identify customer-specific information
    /// in Change Data Capture (CDC) event messages within the Kafka integration module.
    /// This constant helps to standardize the retrieval of customer identifiers from message headers.
    /// </summary>
    public const string CustomerIdHeaderKey = "customerId";
}