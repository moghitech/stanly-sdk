using Fusion.Messaging.RabbitMq.Abstractions;
using MassTransit;

namespace Fusion.Messaging.RabbitMq.Internals.MassTransit;

/// <inheritdoc cref="IMessagingRegistrationContext"/>
internal class MassTransitRegistrationContext(IBusRegistrationContext inner) : IMessagingRegistrationContext
{
    internal readonly IBusRegistrationContext Inner = inner;
}