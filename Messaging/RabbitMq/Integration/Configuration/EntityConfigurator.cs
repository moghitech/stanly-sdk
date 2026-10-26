using Fusion.Messaging.RabbitMq.Integration.Abstractions;
using Fusion.Messaging.RabbitMq.Integration.Mappers;

namespace Fusion.Messaging.RabbitMq.Integration.Configuration;

/// <summary>
/// Defines non-generic access to entity configuration factories.
/// </summary>
public interface IEntityConfigurator
{
    /// <summary>
    /// A delegate function for creating an event or object from an entity during creation.
    /// </summary>
    Func<object, IEntityEvent>? CreatedFactory { get; }

    /// <summary>
    /// A delegate function for updating an event or object from an entity during update.
    /// </summary>
    Func<object, IEntityEvent>? UpdatedFactory { get; }

    /// <summary>
    /// A delegate function for handling deletion of an entity.
    /// </summary>
    Func<object, IEntityEvent>? DeletedFactory { get; }
}

/// <summary>
/// Configures entity-specific behaviors for creating, updating, and deleting events,
/// enabling integration with event mapping and factory creation.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity to be configured. Must be a reference type.
/// </typeparam>
public class EntityConfigurator<TEntity> : IEntityConfigurator where TEntity : class
{
    private Func<TEntity, IEntityEvent>? CreatedFactoryTyped { get; set; }
    private Func<TEntity, IEntityEvent>? UpdatedFactoryTyped { get; set; }
    private Func<TEntity, IEntityEvent>? DeletedFactoryTyped { get; set; }

    /// <summary>
    /// Configures behavior for the creation event of the specified entity type,
    /// enabling custom mapping between the entity and an event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">
    /// The type of the event associated with the creation of the entity. Must be a reference type and have a parameterless constructor.
    /// </typeparam>
    /// <param name="configureMap">
    /// An action that defines how to map properties between the entity of type <typeparamref name="TEntity"/>
    /// and the event of type <typeparamref name="TEvent"/> using an <see cref="EventMapBuilder{TEntity, TEvent}"/>.
    /// </param>
    /// <returns>
    /// The current <see cref="EntityConfigurator{TEntity}"/> instance, allowing for method chaining.
    /// </returns>
    public EntityConfigurator<TEntity> OnCreated<TEvent>(Action<EventMapBuilder<TEntity, TEvent>> configureMap)
        where TEvent : class, new()
    {
        var builder = new EventMapBuilder<TEntity, TEvent>();
        configureMap(builder);
        CreatedFactoryTyped = e => (IEntityEvent)builder.BuildFactory()(e);
        return this;
    }

    /// <summary>
    /// Configures behavior for the update event of the specified entity type,
    /// enabling custom mapping between the entity and an event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">
    /// The type of the event associated with the update of the entity. Must be a reference type and have a parameterless constructor.
    /// </typeparam>
    /// <param name="configureMap">
    /// An action that defines how to map properties between the entity of type <typeparamref name="TEntity"/>
    /// and the event of type <typeparamref name="TEvent"/> using an <see cref="EventMapBuilder{TEntity, TEvent}"/>.
    /// </param>
    /// <returns>
    /// The current <see cref="EntityConfigurator{TEntity}"/> instance, allowing for method chaining.
    /// </returns>
    public EntityConfigurator<TEntity> OnUpdated<TEvent>(Action<EventMapBuilder<TEntity, TEvent>> configureMap)
        where TEvent : class, new()
    {
        var builder = new EventMapBuilder<TEntity, TEvent>();
        configureMap(builder);
        UpdatedFactoryTyped = e => (IEntityEvent)builder.BuildFactory()(e);
        return this;
    }

    /// <summary>
    /// Configures behavior for the deletion event of the specified entity type,
    /// enabling custom mapping between the entity and an event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">
    /// The type of the event associated with the deletion of the entity. Must be a reference type and have a parameterless constructor.
    /// </typeparam>
    /// <param name="configureMap">
    /// An action that defines how to map properties between the entity of type <typeparamref name="TEntity"/>
    /// and the event of type <typeparamref name="TEvent"/> using an <see cref="EventMapBuilder{TEntity, TEvent}"/>.
    /// </param>
    /// <returns>
    /// The current <see cref="EntityConfigurator{TEntity}"/> instance, allowing for method chaining.
    /// </returns>
    public EntityConfigurator<TEntity> OnDeleted<TEvent>(Action<EventMapBuilder<TEntity, TEvent>> configureMap)
        where TEvent : class, new()
    {
        var builder = new EventMapBuilder<TEntity, TEvent>();
        configureMap(builder);
        DeletedFactoryTyped = e => (IEntityEvent)builder.BuildFactory()(e);
        return this;
    }

    Func<object, IEntityEvent>? IEntityConfigurator.CreatedFactory =>
        CreatedFactoryTyped != null ? obj => CreatedFactoryTyped((TEntity)obj) : null;

    Func<object, IEntityEvent>? IEntityConfigurator.UpdatedFactory =>
        UpdatedFactoryTyped != null ? obj => UpdatedFactoryTyped((TEntity)obj) : null;

    Func<object, IEntityEvent>? IEntityConfigurator.DeletedFactory =>
        DeletedFactoryTyped != null ? obj => DeletedFactoryTyped((TEntity)obj) : null;
}