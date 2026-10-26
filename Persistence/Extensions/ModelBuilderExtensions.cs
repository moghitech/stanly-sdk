using System.Linq.Expressions;
using System.Reflection;
using Fusion.Entities;
using Fusion.Entities.Auditable;
using Fusion.Entities.Contracts;
using Fusion.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fusion.Persistence.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ModelBuilder"/> class to simplify
/// the registration of entity type configurations and entities implementing the <see cref="IEntity"/> interface.
/// </summary>
public static class ModelBuilderExtensions
{
    
    private const string ReadModelSchemaName = "readmodel";
    
    /// <summary>
    /// Registers entity type configurations and all entities that implement the <see cref="IEntity"/> interface
    /// from the assembly containing the <see cref="TDbContext"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ModelBuilder"/> instance used to configure the database model.</param>
    public static void RegisterTypeConfigurations<TDbContext>(this ModelBuilder builder) where TDbContext : DbContext
    {
        var dbContextAssembly = typeof(TDbContext).Assembly;
        builder.RegisterAllEntities<IEntity>(dbContextAssembly);
        builder.RegisterEntityTypeConfiguration(dbContextAssembly);
    }

    /// <summary>
    /// Applies query filters for soft delete functionality to all entities implementing the <see cref="IAuditableEntity{TKey}"/> interface
    /// in the provided <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance used to configure the database model.</param>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    public static void ApplyAuditionSoftDeleteQueryFilters<TDbContext>(this ModelBuilder modelBuilder)
    {
        var auditableEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(et => typeof(IAuditableEntity<long>).IsAssignableFrom(et.ClrType))
            .ToList();

        foreach (var entityType in auditableEntityTypes)
        {
            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            var filter = method.Invoke(null, null);
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter((LambdaExpression)filter!);
        }
    }

    /// <summary>
    /// Registers all non-abstract classes that represent base read model entities, mapping them to tables
    /// in the "readmodel" schema within the provided <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance used to configure the database model.</param>
    public static void RegisterBaseReadModelEntities(this ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        var baseReadModelTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && IsReadModelEntity(t))
            .ToList();

        foreach (var type in baseReadModelTypes)
        {
            modelBuilder.Entity(type).ToTable(type.Name, ReadModelSchemaName);
        }
    }

    private static bool IsReadModelEntity(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(ReadModelEntity<>))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : class, IAuditableEntity<long>
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var property = Expression.Property(parameter, nameof(IAuditableEntity<long>.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda<Func<TEntity, bool>>(condition, parameter);
    }
}