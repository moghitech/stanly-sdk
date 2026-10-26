namespace Fusion.Microservice.Models;

/// <summary>
/// Represents the input viewModel for pagination.
/// </summary>
/// <param name="Take"> The maximum number of items to retrieve per page</param>
/// <param name="Skip">The number of items to skip from the beginning of the list</param>
public abstract record PaginationFilter(
    int Take,
    int Skip
);
