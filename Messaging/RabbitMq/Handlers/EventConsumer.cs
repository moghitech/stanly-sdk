using System.Runtime.CompilerServices;
using Fusion.Messaging.RabbitMq.Integration.Abstractions;
using Fusion.Messaging.RabbitMq.Integration.Helpers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fusion.Messaging.RabbitMq.Handlers;

/// <summary>
/// Represents a consumer for handling events related to student panel operations.
/// </summary>
/// <remarks>
/// This class is designed to consume and process events of type <see cref="BaseEntityEvent"/>
/// in the context of student panel infrastructure messaging. It implements the
/// </remarks>
public sealed class EventConsumer(IEventHandlerStrategy eventHandlerStrategy, ILogger<EventConsumer> logger)
    : IConsumer<BaseEntityEvent>
{
    private const string LogKey = nameof(EventConsumer);

    /// <summary>
    /// Consumes and handles the incoming <see cref="BaseEntityEvent"/>.
    /// </summary>
    /// <param name="context">The consumer context containing the message to process.</param>
    public async Task Consume(ConsumeContext<BaseEntityEvent> context)
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
                var @event = context.Message.ToEntityEvent();
                if (@event is null) return;

                var handler = eventHandlerStrategy.GetHandler(context.Message.EventTypeFullName);
                if (handler is null) return;

                await handler.HandleAsync(@event, context.CancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(
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