namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Transport-level paging options coming from API/UI. 
/// Keep domain querying (Specification) independent; map this to Skip/Take there.
/// </summary>
public sealed record PageRequest
{
    /// <summary>1-based page number (minimum 1).</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>Requested page size (will be clamped to [1..MaxPageSize]).</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Upper bound for <see cref="PageSize"/> to protect the system.
    /// You may override this per-request if needed, but keep a sane default.
    /// </summary>
    public int MaxPageSize { get; init; } = 100;

    /// <summary>Calculated number of items to skip (never negative).</summary>
    public int Skip
    {
        get
        {
            var page = PageNumber < 1 ? 1 : PageNumber;
            var size = PageSize < 1 ? 1 : (PageSize > MaxPageSize ? MaxPageSize : PageSize);
            var skip = (page - 1) * size;
            return skip < 0 ? 0 : skip;
        }
    }

    /// <summary>Calculated number of items to take after clamping.</summary>
    public int Take
    {
        get
        {
            var size = PageSize < 1 ? 1 : (PageSize > MaxPageSize ? MaxPageSize : PageSize);
            return size;
        }
    }
}
