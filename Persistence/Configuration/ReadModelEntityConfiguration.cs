using Fusion.Entities.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fusion.Persistence.Configuration;

/// <summary>
/// Provides configuration for read-model entities in the database, defining common properties and constraints
/// such as keys, timestamps, and indexing for efficiently managing read-oriented data models.
/// </summary>
/// <typeparam name="TEntity">The type of the read-model entity being configured.</typeparam>
/// <typeparam name="TKey">The type of the primary key used by the entity.</typeparam>
public abstract class ReadModelEntityConfiguration<TEntity, TKey> where TEntity : ReadModelEntity<TKey>
{
    /// <summary>
    /// Configures base demand configuration of all TEntity objects.
    /// </summary>
    /// <param name="builder"></param>
    protected void BaseConfigure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(property => property.Id);

        builder.Property(property => property.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}