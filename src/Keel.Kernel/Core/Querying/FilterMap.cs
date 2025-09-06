using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Whitelist/builder to safely map transport-level field filters to specification predicates.
/// </summary>
public sealed class FilterMap<TEntity> where TEntity : class
{
    public delegate bool TryParseFunc<T>(string input, out T value);

    // Key format: "field|op" (case-insensitive on field)
    private readonly Dictionary<string, Action<Specification<TEntity>, FieldFilter>> _map =
        new(StringComparer.OrdinalIgnoreCase);

    private static string Key(string field, FilterOperator op) => $"{field}|{(int)op}";

    /// <summary>Registers an Equals mapping for any property type.</summary>
    public FilterMap<TEntity> ForEquals<TProp>(
        string field,
        Expression<Func<TEntity, TProp>> selector,
        TryParseFunc<TProp> parse) where TProp : notnull
    {
        _map[Key(field, FilterOperator.Equals)] = (spec, term) =>
        {
            if (string.IsNullOrWhiteSpace(term.Value)) return;
            if (!parse(term.Value!, out var value)) return;

            var param = selector.Parameters[0];
            var body = Expression.Equal(selector.Body, Expression.Constant(value, typeof(TProp)));
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);
            spec.Where(lambda);
        };
        return this;
    }

    /// <summary>Registers a case-(in)sensitive string equality mapping.</summary>
    public FilterMap<TEntity> ForStringEquals(
        string field,
        Expression<Func<TEntity, string?>> selector,
        bool caseInsensitive = true,
        bool trimInput = true)
    {
        _map[Key(field, FilterOperator.Equals)] = (spec, term) =>
        {
            if (string.IsNullOrWhiteSpace(term.Value)) return;
            var v = trimInput ? term.Value!.Trim() : term.Value!;

            var param = selector.Parameters[0];
            var member = selector.Body; // string?

            Expression body;
            if (caseInsensitive)
            {
                // member != null && member.ToLower().Equals(v.ToLower())
                var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                var equals = typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string) })!;
                var left = Expression.Call(member!, toLower);
                var right = Expression.Constant(v.ToLower());
                var eqCall = Expression.Call(left, equals, right);
                var notNull = Expression.NotEqual(member!, Expression.Constant(null, typeof(string)));
                body = Expression.AndAlso(notNull, eqCall);
            }
            else
            {
                var equals = typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string) })!;
                var right = Expression.Constant(v, typeof(string));
                var notNull = Expression.NotEqual(member!, Expression.Constant(null, typeof(string)));
                var eqCall = Expression.Call(member!, equals, right);
                body = Expression.AndAlso(notNull, eqCall);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);
            spec.Where(lambda);
        };
        return this;
    }

    /// <summary>Registers a Contains mapping for string properties.</summary>
    public FilterMap<TEntity> ForContains(
        string field,
        Expression<Func<TEntity, string?>> selector,
        bool caseInsensitive = true,
        bool trimInput = true)
    {
        _map[Key(field, FilterOperator.Contains)] = (spec, term) =>
        {
            if (string.IsNullOrWhiteSpace(term.Value)) return;
            var needle = trimInput ? term.Value!.Trim() : term.Value!;

            var param = selector.Parameters[0];
            var member = selector.Body; // string?

            // member != null && member.ToLower().Contains(needle.ToLower())
            var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

            Expression left = member!;
            Expression rhs = Expression.Constant(needle);

            if (caseInsensitive)
            {
                left = Expression.Call(left, toLower);
                rhs = Expression.Constant(needle.ToLower());
            }

            var containsCall = Expression.Call(left, contains, rhs);
            var notNull = Expression.NotEqual(member!, Expression.Constant(null, typeof(string)));
            var body = Expression.AndAlso(notNull, containsCall);

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);
            spec.Where(lambda);
        };
        return this;
    }

    /// <summary>Registers a Between mapping for comparable value types (numbers/dates).</summary>
    public FilterMap<TEntity> ForBetween<TProp>(
        string field,
        Expression<Func<TEntity, TProp>> selector,
        TryParseFunc<TProp> parse)
        where TProp : struct, IComparable<TProp>
    {
        _map[Key(field, FilterOperator.Between)] = (spec, term) =>
        {
            // Parse at least one bound
            TProp? from = null, to = null;
            if (!string.IsNullOrWhiteSpace(term.From) && parse(term.From!, out var f)) from = f;
            if (!string.IsNullOrWhiteSpace(term.To) && parse(term.To!, out var t)) to = t;

            if (from is null && to is null) return;

            var range = new RangeFilter<TProp>
            {
                From = from,
                To = to,
                FromInclusive = term.FromInclusive,
                ToInclusive = term.ToInclusive
            };

            spec.ApplyRange(range, selector);
        };
        return this;
    }

    internal bool TryGet(string field, FilterOperator op, out Action<Specification<TEntity>, FieldFilter> apply)
        => _map.TryGetValue(Key(field, op), out apply!);
}
