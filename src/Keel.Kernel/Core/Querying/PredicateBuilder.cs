using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Fluent helper to incrementally build predicates for specifications.
/// </summary>
public static class PredicateBuilder
{
    /// <summary>Creates a predicate that always returns true.</summary>
    public static Expression<Func<T, bool>> True<T>() => _ => true;

    /// <summary>Creates a predicate that always returns false.</summary>
    public static Expression<Func<T, bool>> False<T>() => _ => false;

    /// <summary>Combines two predicates via logical AND.</summary>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right) => left.AndAlso(right);

    /// <summary>Combines two predicates via logical OR.</summary>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right) => left.OrElse(right);
}
