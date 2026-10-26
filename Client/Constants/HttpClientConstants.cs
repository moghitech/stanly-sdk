namespace Fusion.Client.Constants;

internal static class HttpClientConstants
{
    /// <summary>
    /// this time will be multiplied by retry count
    /// </summary>
    public const int BaseRetryDelayInMillisecond = 100;
    public const int RetryCount = 3;
    
    public const int CircuitBreakerHandledEventsAllowedBeforeBreaking = 5;
    public const int DurationOfBreakInSecond = 10;
}