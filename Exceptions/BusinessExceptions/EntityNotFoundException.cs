namespace Fusion.Exceptions.BusinessExceptions;

/// <inheritdoc cref="NotFoundException"/>
public class EntityNotFoundException<TKey> : NotFoundException
{
    /// <inheritdoc cref="NotFoundException"/>
    public EntityNotFoundException(string entityName, TKey id) : base($"There is no any {entityName} with ID '{id}'")
    {
        EntityName = entityName;
        Ids = new Dictionary<string, TKey> { { "ID", id } };
    }

    /// <inheritdoc cref="NotFoundException"/>
    public EntityNotFoundException(string entityName, Dictionary<string, TKey> ids) : base(
        $"No instance of \"{entityName}\" was found with the following field(s) and value(s): {FormatIds(ids)}")
    {
        EntityName = entityName;
        Ids = ids ?? throw new ArgumentNullException(nameof(ids));
    }

    /// <summary>
    /// Gets the name of the entity associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property holds the name of the entity that could not be found, which is used to provide context for the error message.
    /// It typically reflects the type or logical name of the entity in question.
    /// </remarks>
    public string EntityName { get; }

    /// <summary>
    /// Gets the dictionary of key-value pairs where keys represent field names and values represent the associated ID(s)
    /// for the entity that could not be found.
    /// </summary>
    public Dictionary<string, TKey> Ids { get; }

    private static string FormatIds(Dictionary<string, TKey> ids)
    {
        return ids.Count is 0
            ? "None"
            : string.Join(", ", ids.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }
}

/// <inheritdoc cref="EntityNotFoundException"/>
public class EntityNotFoundException : EntityNotFoundException<long>
{
    /// <inheritdoc cref="EntityNotFoundException"/>
    public EntityNotFoundException(string entityName, long id) : base(entityName, id)
    {
    }

    /// <inheritdoc cref="EntityNotFoundException"/>
    public EntityNotFoundException(string entityName, Dictionary<string, long> ids) : base(entityName, ids)
    {
    }
}