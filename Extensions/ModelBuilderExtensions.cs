using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Fusion.Extensions;

public static class ModelBuilderExtensions
{
    public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        var applyGenericMethod = typeof(ModelBuilder).GetMethods()
            .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

        var types = assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(c => c.IsClass && c is { IsAbstract: false, IsPublic: true });

        foreach (var type in types)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsConstructedGenericType ||
                    @interface.GetGenericTypeDefinition() != typeof(IEntityTypeConfiguration<>)) continue;
                var applyConcreteMethod =
                    applyGenericMethod.MakeGenericMethod(@interface.GenericTypeArguments[0]);
                applyConcreteMethod.Invoke(modelBuilder, [Activator.CreateInstance(type)]);
            }
        }
    }

    public static void RegisterAllEntities<TBaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(c => c.IsClass && c is { IsAbstract: false, IsPublic: true } &&
                        typeof(TBaseType).IsAssignableFrom(c));

        foreach (var type in types)
            modelBuilder.Entity(type);
    }
}