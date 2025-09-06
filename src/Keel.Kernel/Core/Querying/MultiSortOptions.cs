namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Transport-level multi-sort options. Use with a whitelist map to expressions.
/// </summary>
public sealed record MultiSortOptions
{
    public sealed record SortField(string Field, bool Desc = false);

    /// <summary>Ordered list of sort fields (first has highest precedence).</summary>
    public IReadOnlyList<SortField> Fields { get; init; } = Array.Empty<SortField>();

    public bool HasAny => Fields.Count > 0;

    /// <summary>
    /// Parses a simple comma-separated syntax: "firstName,-createdOn,department".
    /// A leading '-' means descending.
    /// </summary>
    public static MultiSortOptions Parse(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv)) return new MultiSortOptions();
        var parts = csv.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var list = new List<SortField>(parts.Length);
        foreach (var p in parts)
        {
            var desc = p.StartsWith("-");
            var name = desc ? p[1..] : p;
            if (!string.IsNullOrWhiteSpace(name))
                list.Add(new SortField(name, desc));
        }
        return new MultiSortOptions { Fields = list };
    }
}
