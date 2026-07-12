using System.Text.Json.Serialization;

namespace ProgressTrackingService.Common.Pagination;

/// <summary>
/// A generic wrapper to encapsulate paginated data along with its metadata.
/// </summary>
public class PaginatedResult<TEntity>
{
    /// <summary>
    /// Total number of pages calculated based on TotalCount and PageSize (25 / 10 = 3 pages).
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Number of records displayed per page (PageSize = 10).
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// The current active page number requested by the user (1-indexed).
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// Total number of records matching the query in the database (TotalCount = 25).
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The actual data subset (list of entities/DTOs) for the current page.
    /// </summary>
    public List<TEntity> Data { get; set; }
    
    [JsonConstructor]
    public PaginatedResult()
    {
    }
    
    public PaginatedResult(List<TEntity> data, int totalCount, int pageNumber, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        
        // Math.Ceiling ensures any fractional page creates a whole new page (e.g., 2.5 becomes 3)
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}