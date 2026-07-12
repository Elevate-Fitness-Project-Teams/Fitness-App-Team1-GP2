using Microsoft.EntityFrameworkCore;

namespace ProgressTrackingService.Common.Pagination;

public static class PaginationExtensions
{
    /// <summary>
    /// This helper method splits any data list into smaller pages.
    /// </summary>
    public static async Task<PaginatedResult<TEntity>> ToPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> source,
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        // 1. If page number or size is less than 1, set safe default values.
        // Max page size allowed is 100 to protect the database performance.
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

        // 2. Count the total number of items in the database.
        var totalCount = await source.CountAsync(cancellationToken);

        // 3. Skip the old items and take only the items for the current page.
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // 4. Return the data wrapped inside the pagination result class.
        return new PaginatedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }
}