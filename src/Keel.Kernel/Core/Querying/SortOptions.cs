namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Transport-level sorting options. Map these to Specification.OrderBy in the Application layer
/// via a whitelist (field-name -> key selector expression) to avoid invalid/unsafe fields.
/// </summary>
public sealed record SortOptions
{
    /// <summary>
    /// Name of the field to order by (as used by API/UI).
    /// Must be validated/mapped to a real key selector expression at the Application layer.
    /// </summary>
    public string? OrderBy { get; init; }

    /// <summary>Descending if true; ascending otherwise.</summary>
    public bool Desc { get; init; } = false;

    /// <summary>Returns true when a sort field is provided.</summary>
    public bool HasSort => !string.IsNullOrWhiteSpace(OrderBy);
}
