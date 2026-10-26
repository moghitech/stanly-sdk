using System.Linq.Expressions;
using System.Reflection;
using Fusion.Messaging.RabbitMq.Integration.Abstractions;

namespace Fusion.Messaging.RabbitMq.Integration.Mappers;

/// <summary>
/// Provides functionality to build mappings between an entity of type <typeparamref name="TEntity"/>
/// and an event of type <typeparamref name="TEvent"/>. This enables property-level mapping to translate
/// entity properties into corresponding event properties during event creation.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity to map from. Must be a reference type.
/// </typeparam>
/// <typeparam name="TEvent">
/// The type of the event to map to. Must be a reference type and include a parameterless constructor.
/// </typeparam>
public class EventMapBuilder<TEntity, TEvent>
    where TEntity : class
    where TEvent : class, new()
{
    private readonly List<Action<TEntity, TEvent>> _mappers = [];

    /// <summary>
    /// Maps a source property from an entity to a target property on an event.
    /// </summary>
    /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
    /// <param name="sourceProperty">
    /// An expression representing the property on the entity to map from.
    /// </param>
    /// <param name="targetProperty">
    /// An expression representing the property on the event to map to.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the target property does not have a setter.
    /// </exception>
    public void Map<TProp>(
        Expression<Func<TEntity, TProp>> sourceProperty,
        Expression<Func<TEvent, TProp>> targetProperty)
    {
        var sourceGetter = sourceProperty.Compile();

        var targetPropInfo = GetPropertyInfo(targetProperty);

        var targetSetter = (Action<TEvent, TProp>)Delegate.CreateDelegate(
            typeof(Action<TEvent, TProp>),
            targetPropInfo.GetSetMethod() ??
            throw new InvalidOperationException($"Property {targetPropInfo.Name} must have a setter.")
        );

        _mappers.Add((source, target) => targetSetter(target, sourceGetter(source)));
    }

    /// <summary>
    /// Builds and returns a factory function that maps an entity of type <typeparamref name="TEntity"/>
    /// to an event of type <typeparamref name="TEvent"/> based on the configured property mappings.
    /// </summary>
    /// <returns>
    /// A function that, given an entity of type <typeparamref name="TEntity"/>, produces an event of type <typeparamref name="TEvent"/>
    /// with its properties populated according to the configured mappings.
    /// </returns>
    public Func<TEntity, TEvent> BuildFactory()
    {
        return entity =>
        {
            var evt = new TEvent();
            foreach (var mapper in _mappers)
            {
                mapper(entity, evt);
            }

            if (evt is IEntityEvent entityEvent)
            {
                entityEvent.Timestamp = DateTime.UtcNow;
            }

            return evt;
        };
    }

    /// <summary>
    /// Retrieves the <see cref="PropertyInfo"/> object for a property based on a provided lambda expression.
    /// </summary>
    /// <typeparam name="TProp">The type of the property to retrieve information for.</typeparam>
    /// <param name="propertyLambda">
    /// A lambda expression representing the property. The body of the expression must refer to a property on the type <typeparamref name="TEvent"/>.
    /// </param>
    /// <returns>
    /// The <see cref="PropertyInfo"/> of the property represented by the lambda expression.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided expression does not refer to a property or refers to a method instead.
    /// </exception>
    private static PropertyInfo GetPropertyInfo<TProp>(Expression<Func<TEvent, TProp>> propertyLambda)
    {
        if (propertyLambda.Body is MemberExpression member)
        {
            return (PropertyInfo)member.Member;
        }

        throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
    }
}