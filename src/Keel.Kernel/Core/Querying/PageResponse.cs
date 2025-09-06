namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Standard paged response payload returning items and counters.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed record PageResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }

    /// <summary>Total pages computed from count and size (at least 1 if there are items; 0 when empty).</summary>
    public int TotalPages
    {
        get
        {
            if (TotalCount <= 0 || PageSize <= 0) return 0;
            var pages = TotalCount / PageSize + (TotalCount % PageSize > 0 ? 1 : 0);
            return pages;
        }
    }
}
