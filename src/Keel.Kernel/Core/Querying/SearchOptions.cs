namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Transport-level filter/search options (minimal).
/// Compose into Specification.Criteria at the Application layer.
/// </summary>
public sealed record SearchOptions
{
    /// <summary>
    /// Free-text search term (case/diacritics normalization should be handled at Application/Data layer).
    /// </summary>
    public string? SearchTerm { get; init; }
}
