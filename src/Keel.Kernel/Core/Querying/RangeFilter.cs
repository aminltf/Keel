namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Inclusive/exclusive range filter for comparable value types (numbers, dates).
/// Example: From=2024-01-01 (inclusive), To=2024-12-31 (exclusive).
/// </summary>
public readonly record struct RangeFilter<T> where T : struct, IComparable<T>
{
    public RangeFilter()
    {
    }

    /// <summary>Lower bound (null means unbounded).</summary>
    public T? From { get; init; }

    /// <summary>If true, value >= From; otherwise value &gt; From.</summary>
    public bool FromInclusive { get; init; } = true;

    /// <summary>Upper bound (null means unbounded).</summary>
    public T? To { get; init; }

    /// <summary>If true, value &lt;= To; otherwise value &lt; To.</summary>
    public bool ToInclusive { get; init; } = true;

    public bool IsEmpty => From is null && To is null;

    /// <summary>Checks if a value is within the configured range.</summary>
    public bool Contains(T value)
    {
        if (From.HasValue)
        {
            var cmp = value.CompareTo(From.Value);
            if (FromInclusive ? cmp < 0 : cmp <= 0) return false;
        }
        if (To.HasValue)
        {
            var cmp = value.CompareTo(To.Value);
            if (ToInclusive ? cmp > 0 : cmp >= 0) return false;
        }
        return true;
    }

    public static RangeFilter<T> Empty => new();
}
