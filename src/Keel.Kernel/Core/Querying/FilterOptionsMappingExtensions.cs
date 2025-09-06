namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Applies transport-level <see cref="FilterOptions"/> onto a specification using a whitelist-based <see cref="FilterMap{TEntity}"/>.
/// Unknown fields or unparsable values are silently ignored.
/// </summary>
public static class FilterOptionsMappingExtensions
{
    public static void ApplyFilters<TEntity>(
        this Specification<TEntity> spec,
        FilterOptions filters,
        Action<FilterMap<TEntity>> configureMap)
        where TEntity : class
    {
        if (filters is not { HasAny: true }) return;

        var map = new FilterMap<TEntity>();
        configureMap(map);

        foreach (var term in filters.Filters)
        {
            if (string.IsNullOrWhiteSpace(term.Field)) continue;
            if (!map.TryGet(term.Field, term.Operator, out var apply)) continue;

            apply(spec, term);
        }
    }
}
