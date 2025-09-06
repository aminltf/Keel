using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Utilities for composing expression trees (useful when building complex specifications).
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>Combines two expressions with a logical AND (parameter-rebinding safe).</summary>
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = left.Parameters[0];
        var visitor = new ReplaceParameterVisitor(right.Parameters[0], param);
        var body = Expression.AndAlso(left.Body, visitor.Visit(right.Body)!);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>Combines two expressions with a logical OR (parameter-rebinding safe).</summary>
    public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = left.Parameters[0];
        var visitor = new ReplaceParameterVisitor(right.Parameters[0], param);
        var body = Expression.OrElse(left.Body, visitor.Visit(right.Body)!);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == source ? target : base.VisitParameter(node);
    }
}
