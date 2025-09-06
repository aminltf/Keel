using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Safe mapping helpers to apply transport-level multi-sort options onto a specification,
/// using a whitelist of permissible fields.
/// </summary>
public static class SortMappingExtensions
{
    /// <summary>
    /// Applies multi-sort options to the specification using a whitelist map:
    ///   uiFieldName -> key selector expression.
    /// Unknown fields are ignored. If no valid field is provided, default ordering (if any) is applied.
    /// </summary>
    public static void ApplySorts<TEntity>(
        this Specification<TEntity> spec,
        MultiSortOptions sort,
        IReadOnlyDictionary<string, Expression<Func<TEntity, object>>> whitelist,
        IEnumerable<(Expression<Func<TEntity, object>> Key, bool Desc)>? defaultOrder = null)
        where TEntity : class
    {
        var any = false;
        if (sort is { HasAny: true })
        {
            foreach (var f in sort.Fields)
            {
                if (whitelist.TryGetValue(f.Field, out var key))
                {
                    if (f.Desc) spec.AddOrderByDesc(key);
                    else spec.AddOrderByAsc(key);
                    any = true;
                }
            }
        }

        if (!any && defaultOrder is not null)
        {
            foreach (var (key, desc) in defaultOrder)
            {
                if (desc) spec.AddOrderByDesc(key);
                else spec.AddOrderByAsc(key);
            }
        }
    }
}
