using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fusion.Persistence.Converters;

/// <summary>
/// Provides functionality for configuring JSON serialization and storage for entity properties in an Entity Framework Core model.
/// </summary>
public static class JsonColumnConverter
{
    /// <summary>
    /// Configures the entity property to use JSON serialization for storage in the database.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be stored as JSON.</typeparam>
    /// <param name="builder">The <see cref="PropertyBuilder{TProperty}"/> for configuring the entity property.</param>
    public static void HasJsonColumns<TProperty>(this PropertyBuilder<TProperty> builder)
    {
        builder
            .HasConversion(property => JsonSerializer.Serialize(property, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<TProperty>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb").Metadata.SetValueComparer(new ValueComparer<TProperty>(
                (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) ==
                            JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                c => JsonSerializer.Deserialize<TProperty>(
                    JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
            ));
    }
}