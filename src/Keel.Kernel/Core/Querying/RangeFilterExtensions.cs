using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Helpers to turn <see cref="RangeFilter{T}"/> into predicates and to apply them to specifications.
/// </summary>
public static class RangeFilterExtensions
{
    /// <summary>
    /// Builds an expression like:
    ///   e => (e.Prop >= from) && (e.Prop &lt; to)
    /// respecting inclusivity for both ends.
    /// </summary>
    public static Expression<Func<TEntity, bool>> ToPredicate<TEntity, TProp>(
        this RangeFilter<TProp> range,
        Expression<Func<TEntity, TProp>> selector)
        where TProp : struct, IComparable<TProp>
    {
        var param = selector.Parameters[0];
        Expression? body = null;

        var member = selector.Body; // e.Prop

        if (range.From is not null)
        {
            var fromConst = Expression.Constant(range.From.Value, typeof(TProp));
            var lower = range.FromInclusive
                ? Expression.GreaterThanOrEqual(member, fromConst)
                : Expression.GreaterThan(member, fromConst);
            body = lower;
        }

        if (range.To is not null)
        {
            var toConst = Expression.Constant(range.To.Value, typeof(TProp));
            var upper = range.ToInclusive
                ? Expression.LessThanOrEqual(member, toConst)
                : Expression.LessThan(member, toConst);
            body = body is null ? upper : Expression.AndAlso(body, upper);
        }

        // If empty, return 'true'
        body ??= Expression.Constant(true);
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    /// <summary>
    /// Adds a range filter to an existing predicate: <c>criteria AND rangePredicate</c>.
    /// </summary>
    public static Expression<Func<TEntity, bool>> AndRange<TEntity, TProp>(
        this Expression<Func<TEntity, bool>> criteria,
        RangeFilter<TProp> range,
        Expression<Func<TEntity, TProp>> selector)
        where TProp : struct, IComparable<TProp>
    {
        var rangeExpr = range.ToPredicate(selector);
        return criteria.AndAlso(rangeExpr);
    }

    /// <summary>
    /// Applies a range filter to a <see cref="Specification{T}"/> by AND-combining with its current criteria.
    /// </summary>
    public static void ApplyRange<TEntity, TProp>(
        this Specification<TEntity> spec,
        RangeFilter<TProp> range,
        Expression<Func<TEntity, TProp>> selector)
        where TEntity : class
        where TProp : struct, IComparable<TProp>
    {
        if (range.IsEmpty) return;
        var pred = range.ToPredicate(selector);
        spec.Where(pred);
    }
}
