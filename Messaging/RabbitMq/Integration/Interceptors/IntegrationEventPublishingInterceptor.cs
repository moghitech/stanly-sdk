using System.Runtime.CompilerServices;
using Fusion.Messaging.RabbitMq.Events;
using Fusion.Messaging.RabbitMq.Integration.Configuration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Fusion.Messaging.RabbitMq.Integration.Interceptors;

/// <summary>
/// Interceptor for handling the publishing of integration events during the save changes process in Entity Framework.
/// This class inspects changes made to tracked entities and publishes corresponding events to a message broker
/// based on the entity's state (added, modified, or deleted).
/// </summary>
/// <remarks>
/// The interceptor uses the <see cref="IntegrationConfigurator"/> to determine the appropriate
/// event factory and publishes the events using the <see cref="IPublishEndpoint"/>. Events are
/// generated and published for entities configured with specific lifecycle events.
/// </remarks>
public class IntegrationEventPublishingInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<IntegrationEventPublishingInterceptor> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly IntegrationConfigurator _integrationConfigurator;

    private List<(object Entity, EntityState State)>? _pendingEvents;

    private const string LogKey = nameof(IntegrationEventPublishingInterceptor);

    /// <summary>
    /// Interceptor for publishing integration events based on entity lifecycle changes during the database save operation.
    /// </summary>
    /// <remarks>
    /// This interceptor observes entity state changes (add, update, delete) during the SaveChanges process in Entity Framework
    /// and publishes corresponding integration events to a message broker. It uses the <see cref="IntegrationConfigurator"/>
    /// to determine the appropriate event factory for generating events and employs the <see cref="IPublishEndpoint"/>
    /// to handle event publication.
    /// </remarks>
    public IntegrationEventPublishingInterceptor(
        ILogger<IntegrationEventPublishingInterceptor> logger,
        IEventPublisher eventPublisher,
        IntegrationConfigurator integrationConfigurator
    )
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        _integrationConfigurator = integrationConfigurator;
    }

    /// <summary>
    /// Handles the interception of the SaveChangesAsync operation in Entity Framework to inspect and prepare
    /// entity changes for publishing integration events.
    /// </summary>
    /// <param name="eventData">The event data containing information about the DbContext and its state during the save operation.</param>
    /// <param name="result">The result of the interception logic for the SaveChangesAsync process.</param>
    /// <param name="cancellationToken">A cancellation token to monitor for operation cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the interception result of type <see cref="InterceptionResult{T}"/>.</returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
            return result;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        _pendingEvents = entries.Select(e => (e.Entity, e.State)).ToList();

        await Task.CompletedTask;
        return result;
    }

    /// <summary>
    /// Interceptor method that executes after changes have been saved to the database. This method processes the pending
    /// events triggered by changes in the tracked entities (added, modified, or deleted) and publishes them to a message broker.
    /// </summary>
    /// <param name="eventData">
    /// Contextual information about the SaveChanges operation, including the associated <see cref="DbContext"/> and entity state changes.
    /// </param>
    /// <param name="result">
    /// The result of the SaveChanges operation, representing the number of state entries written to the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that may be utilized to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The final result of the SaveChanges operation, usually the same as the input result, after processing and publishing any
    /// pending integration events.
    /// </returns>
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        var context = eventData.Context;

        if (context is null || _pendingEvents is null)
            return result;

        try
        {
            foreach (var (entity, state) in _pendingEvents)
            {
                var entityType = entity.GetType();

                var factory = _integrationConfigurator.GetFactory(entityType, state);

                var eventMessage = factory?.Invoke(entity);
                if (eventMessage is null) continue;

                _logger.LogInformation(
                    "{LogKey}: Publishing integration event for entity type, [EntityType]=({EntityType}), [EntityState]=({EntityState}), [EventType]=({EventType}).",
                    GetLogKey(), entityType.Name, state, eventMessage.GetType().Name);

                await _eventPublisher.PublishEntityEventAsync(eventMessage, cancellationToken);

                _logger.LogInformation(
                    "Successfully published event. [EventType]=({EventType}), [EntityType]=({EntityType}).",
                    eventMessage.GetType().Name, entityType.Name);
            }
        }
        finally
        {
            _pendingEvents = null;
        }

        return result;
    }

    /// <summary>
    /// Handles the scenario where saving changes to the database fails and ensures
    /// any pending events for publication are cleared.
    /// </summary>
    /// <param name="eventData">The data related to the failed save operation.</param>
    /// <param name="cancellationToken">A cancellation token that may be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken =
        default)
    {
        _pendingEvents = null;
        return Task.CompletedTask;
    }

    private static string GetLogKey([CallerMemberName] string functionName = "") => $"{LogKey}.{functionName}";
}