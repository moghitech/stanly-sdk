using System.Collections.Concurrent;
using Fusion.Messaging.RabbitMq.Integration.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Fusion.Messaging.RabbitMq.Integration.Configuration;

/// <summary>
/// Provides configuration options for integrating entities in messaging services.
/// This class is used to define and manage entity-specific settings for message integration.
/// </summary>
public class IntegrationConfigurator
{
    private readonly ConcurrentDictionary<object, object> _entityConfigs = new();

    /// <summary>
    /// Configures an entity of type <typeparamref name="TEntity"/> with specified actions and mappings.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity to be configured. Must be a reference type.
    /// </typeparam>
    /// <param name="action">
    /// A configuration action that allows the caller to define behavior for entity-specific events,
    /// such as creation, update, or deletion events.
    /// </param>
    /// <returns>
    /// An instance of <see cref="EntityConfigurator{TEntity}"/> to further customize the configuration of the entity.
    /// </returns>
    public EntityConfigurator<TEntity> ConfigureEntity<TEntity>(Action<EntityConfigurator<TEntity>> action)
        where TEntity : class
    {
        var config = new EntityConfigurator<TEntity>();
        action(config);
        _entityConfigs[typeof(TEntity)] = config;
        return config;
    }

    /// <summary>
    /// Retrieves a factory method for producing <see cref="IEntityEvent"/> instances based on the specified entity type and state.
    /// </summary>
    /// <param name="entityType">
    /// The type of the entity for which to retrieve the event factory.
    /// </param>
    /// <param name="state">
    /// The state of the entity (e.g., Added, Modified, or Deleted) based on which the corresponding event factory will be chosen.
    /// </param>
    /// <returns>
    /// A function that takes an object and produces an <see cref="IEntityEvent"/>, or <c>null</c> if no matching factory is found.
    /// </returns>
    internal Func<object, IEntityEvent>? GetFactory(Type entityType, EntityState state)
    {
        if (!_entityConfigs.TryGetValue(entityType, out var configObj)) return null;

        if (configObj is IEntityConfigurator config)
        {
            return state switch
            {
                EntityState.Added => config.CreatedFactory,
                EntityState.Modified => config.UpdatedFactory,
                EntityState.Deleted => config.DeletedFactory,
                _ => null
            };
        }

        return null;
    }
}