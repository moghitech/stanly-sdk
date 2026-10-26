using Fusion.Entities.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fusion.Persistence.Configuration;

/// <summary>
/// Base class configuration for entities.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class BaseEntityConfiguration<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    /// <summary>
    ///  Configures base demand configuration of all TEntity objects.
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void BaseConfigure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(property => property.Id);
    }
}