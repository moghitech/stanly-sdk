using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fusion.Persistence.Interceptors;

/// <summary>
/// An EF Core SaveChangesInterceptor that ensures all DateTime properties being added
/// or modified in the context are converted to UTC before persisting to the database.
/// </summary>
/// <remarks>
/// This interceptor is automatically triggered during the SaveChanges or SaveChangesAsync
/// calls in a DbContext. The interceptor iterates over all tracked entity entries in the
/// DbContext and checks for DateTime properties. If a DateTime property is not in UTC
/// (either unspecified or local), it converts the value to UTC before saving.
/// </remarks>
/// <example>
/// This interceptor should be registered during the DbContext setup to ensure consistent
/// DateTime handling across the application. Used primarily for applications where UTC
/// timezone normalization is required to avoid timezone-related discrepancies in persisted data.
/// </example>
public sealed class DateTimeUtcInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc cref="SaveChangesInterceptor.SavingChanges"/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null) ConvertToUtc(eventData.Context);

        return result;
    }

    /// <inheritdoc cref="SaveChangesInterceptor.SavingChangesAsync"/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null) ConvertToUtc(eventData.Context);

        return new ValueTask<InterceptionResult<int>>(result);
    }

    private static void ConvertToUtc(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified)) continue;

            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
                {
                    property.CurrentValue = dateTime.Kind == DateTimeKind.Local
                        ? dateTime.ToUniversalTime()
                        : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                }
            }
        }
    }
}