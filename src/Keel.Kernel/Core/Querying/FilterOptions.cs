namespace Keel.Kernel.Core.Querying;

/// <summary>
/// A set of transport-level filters supplied by the client/UI.
/// </summary>
public sealed record FilterOptions
{
    public IReadOnlyList<FieldFilter> Filters { get; init; } = Array.Empty<FieldFilter>();
    public bool HasAny => Filters.Count > 0;
}
