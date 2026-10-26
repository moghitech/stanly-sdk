using Fusion.Client.Constants;
using Fusion.Messaging.Kafka.Constants;
using Fusion.Messaging.Kafka.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Fusion.Messaging.Kafka.Debezium.Masstransit;

/// <summary>
/// Represents a message consumption interceptor designed for use with MassTransit consumers
/// that handle events derived from Debezium. This interceptor is responsible for attaching
/// contextual information, such as a customer-specific header, into the HTTP request for downstream processing.
/// </summary>
/// <typeparam name="T">
/// The message type being consumed, which must derive from <see cref="CdcEvent{TLegacyEntity}"/>.
/// </typeparam>
public class ConsumerInterceptor<T>(IHttpContextAccessor accessor) : IFilter<ConsumeContext<T>>
    where T : class
{
    /// <summary>
    /// Sends the current context to the next filter in the pipeline while
    /// appending customer-specific information, such as the Customer ID,
    /// to the HTTP request headers for downstream processing.
    /// </summary>
    /// <param name="context">
    /// The consumption context containing the message and associated metadata.
    /// </param>
    /// <param name="next">
    /// The next pipeline element that processes the consumption context.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of sending
    /// the context down the pipeline.
    /// </returns>
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var customerId = long.Parse(context.Headers.Get<string>(CdcEventConstants.CustomerIdHeaderKey)!);

        accessor.HttpContext ??= new DefaultHttpContext();

        accessor.HttpContext.Request.Headers.Append(HttpRequestHeaderConstants.CustomerId, customerId.ToString());

        await next.Send(context);
    }

    /// <summary>
    /// Creates a diagnostic filter scope for the consumer interceptor, allowing inspection
    /// of the filter's configuration within a pipeline context.
    /// </summary>
    /// <param name="context">
    /// The probe context used for collecting diagnostic information about the filter.
    /// </param>
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(ConsumerInterceptor<T>));
    }
}