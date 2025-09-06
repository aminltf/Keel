namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Transport-level representation of a single field filter.
/// Only one of {Value | (From/To)} should be used depending on the operator.
/// </summary>
public sealed record FieldFilter
{
    /// <summary>The UI/DTO field name (will be mapped via whitelist).</summary>
    public string Field { get; init; } = default!;

    /// <summary>Equals, Contains, or Between.</summary>
    public FilterOperator Operator { get; init; }

    /// <summary>Used by Equals/Contains.</summary>
    public string? Value { get; init; }

    /// <summary>Used by Between: lower bound.</summary>
    public string? From { get; init; }

    /// <summary>Used by Between: upper bound.</summary>
    public string? To { get; init; }

    public bool FromInclusive { get; init; } = true;
    public bool ToInclusive { get; init; } = true;
}
