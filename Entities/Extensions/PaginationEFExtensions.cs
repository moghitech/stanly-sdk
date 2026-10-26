using Fusion.Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace Fusion.Entities.Extensions;

/// <summary>
/// Provides extension methods for handling pagination with IQueryable collections in Entity Framework.
/// </summary>
public static class PaginationEfExtensions
{
    /// <summary>
    /// Paginates an IQueryable collection based on the specified pagination parameters.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the IQueryable collection.</typeparam>
    /// <param name="query">The IQueryable collection to be paginated.</param>
    /// <param name="paginationDto">The pagination parameters, including page size and page number.</param>
    /// <returns>An IQueryable collection representing the paginated result.</returns>
    /// <exception cref="ArgumentException">Thrown when the page size is less than or equal to 0.</exception>
    // public static IQueryable<T> ToPaginated<T>(this IQueryable<T> query, PaginationDto paginationDto)
    public static async Task<Paginated<T>> ToPaginatedAsync<T>(this IQueryable<T> query, PaginationDto paginationDto)
    {
        if (paginationDto.PageSize <= 0)
            throw new ArgumentException("Page size must be positive", nameof(paginationDto.PageSize));
        if (paginationDto.PageNumber < 1)
            throw new ArgumentException("Page number must be positive", nameof(paginationDto.PageNumber));

        var count = await query.CountAsync();

        var result = await query
            .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
            .Take(paginationDto.PageSize)
            .ToListAsync();

        var paginated = new Paginated<T>(
            result,
            count,
            paginationDto.PageNumber,
            paginationDto.PageSize
        );

        return paginated;
    }
}