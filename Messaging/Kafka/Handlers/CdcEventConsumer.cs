using System.Runtime.CompilerServices;
using Fusion.Messaging.Kafka.Constants;
using Fusion.Messaging.Kafka.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fusion.Messaging.Kafka.Handlers;

/// <summary>
/// Internal consumer that bridges MassTransit's IConsumer interface with the framework's
/// ICdcEventHandler interface. This consumer receives messages from Kafka via MassTransit
/// and delegates processing to registered event handlers, providing a decoupled architecture.
/// </summary>
/// <typeparam name="TEvent">The type of CDC event being consumed.</typeparam>
public class CdcEventConsumer<TEvent> : IConsumer<TEvent> where TEvent : class, ICdcEvent
{
    private const string LogKey = nameof(CdcEventConsumer<TEvent>);
    private readonly ICdcEventHandler<TEvent> _handler;
    private readonly ILogger<CdcEventConsumer<TEvent>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CdcEventConsumer{TEvent}"/> class.
    /// </summary>
    /// <param name="handler">The event handler that will process the CDC events.</param>
    /// <param name="logger">The logger instance for logging events and errors.</param>
    public CdcEventConsumer(ICdcEventHandler<TEvent> handler, ILogger<CdcEventConsumer<TEvent>> logger)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _logger = logger;
    }

    /// <summary>
    /// Consumes a message from MassTransit and delegates it to the registered handler.
    /// </summary>
    /// <param name="context">The consumption context containing the message and metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<TEvent> context)
    {
        using (SentrySdk.PushScope())
        {
            SentrySdk.ConfigureScope(scope =>
            {
                scope.SetTag("messaging.message_type", context.Message.GetType().FullName ?? "Unknown");
                scope.SetTag("messaging.correlation_id", context.CorrelationId?.ToString() ?? "none");
                scope.SetTag("messaging.conversation_id", context.ConversationId?.ToString() ?? "none");
                scope.SetTag("messaging.consumer_type", context.ReceiveContext.InputAddress.ToString());
                scope.SetTag("messaging.message_id", context.MessageId?.ToString() ?? "none");
                scope.SetExtra("messaging.headers", context.Headers.GetAll());
                scope.SetExtra("messaging.message", context.Message);
            });

            try
            {
                context.Message.CustomerId =
                    long.Parse(context.Headers.Get<string>(CdcEventConstants.CustomerIdHeaderKey)!);

                await _handler.HandleAsync(context.Message, context.CancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "{LogKey}: Error while processing event in consumer [EventTypeFullName]=({EventTypeFullName}), [Reason]=({Reason}).",
                    GetLogKey(),
                    context.Message.GetType().Name,
                    exception.Message
                );

                SentrySdk.CaptureException(exception);
            }
        }
    }

    private static string GetLogKey([CallerMemberName] string functionName = "") => $"{LogKey}.{functionName}";
}