using Fusion.Entities.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fusion.Persistence.Configuration;

/// <summary>
/// Base class configuration for auditable entities.
/// </summary>
/// <typeparam name="TEntity">The entity</typeparam>
/// <typeparam name="TKey">The entity key</typeparam>
public abstract class AuditableBaseEntityConfiguration<TEntity, TKey> : BaseEntityConfiguration<TEntity, TKey>
    where TEntity : AuditableBaseEntity<TKey>
{
    /// <summary>
    /// Configures base demand configuration of all TEntity objects.
    /// </summary>
    /// <param name="builder"></param>
    protected override void BaseConfigure(EntityTypeBuilder<TEntity> builder)
    {
        base.BaseConfigure(builder);

        builder.Property(property => property.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(property => property.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}