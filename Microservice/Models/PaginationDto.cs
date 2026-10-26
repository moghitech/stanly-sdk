namespace Fusion.Microservice.Models;

/// <summary>
/// Represents a Data Transfer Object (DTO) containing pagination parameters,
/// such as the size of each page and the current page number.
/// </summary>
public record PaginationDto(
    int PageSize,
    int PageNumber
);