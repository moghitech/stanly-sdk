namespace Fusion.Microservice.Models;

/// <summary>
/// Represents a container for paginated data with metadata about the pagination state.
/// </summary>
/// <typeparam name="TData">The type of the data contained in the paginated result.</typeparam>
public class Paginated<TData>
{
    public Paginated()
    {
    }

    /// <summary>
    /// Represents a generic paginated result that contains a list of items, total count, pagination details, and convenience properties for navigation between pages.
    /// </summary>
    /// <typeparam name="TData">The type of data contained within the paginated result.</typeparam>
    public Paginated(List<TData> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = TotalPages is null or 0 ? (int)Math.Ceiling(count / (double)pageSize) : TotalPages;
        TotalCount = TotalCount == 0 ? count : TotalCount;
        Data = items;
    }

    /// <summary>
    /// Represents a generic paginated result that contains a list of items, total count, pagination details, and convenience properties for navigation between pages.
    /// </summary>
    /// <typeparam name="TData">The type of data contained within the paginated result.</typeparam>
    public Paginated(List<TData> items, int count)
    {
        TotalCount = TotalCount == 0 ? count : TotalCount;
        Data = items;
    }

    /// <summary>
    /// Gets or sets the collection of data items in the paginated result.
    /// Represents the list of items retrieved for the current page.
    /// </summary>
    public List<TData> Data { get; set; }

    /// <summary>
    /// Gets or sets the total number of items across all pages in a paginated result.
    /// Represents the complete count of items available, irrespective of the current page.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number in the pagination sequence.
    /// Represents the page being accessed or displayed in the paginated result.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page in the paginated result.
    /// Specifies the size of the page and determines how many items are included in each page of the data.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages in the pagination sequence.
    /// Represents the calculated total number of pages based on the total count and items per page.
    /// </summary>
    public int? TotalPages { get; set; }

    /// <summary>
    /// Indicates whether there is a previous page available in the pagination sequence.
    /// Returns true if the current page number is greater than one, signaling that a prior page exists.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Determines whether there is a subsequent page available in the pagination sequence.
    /// Evaluates if the current page is less than the total number of pages.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
};