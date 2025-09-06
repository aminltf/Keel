using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Encapsulates query logic for an entity type in a reusable, composable way.
/// Providers (EF, Dapper, InMemory) can translate specifications to their query APIs.
/// </summary>
/// <typeparam name="T">The entity type being queried.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Filtering criteria as an expression tree.
    /// Null means "no filtering".
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Navigation properties to include in the query.
    /// Used by ORMs that support eager loading (e.g., EF).
    /// </summary>
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Order-by definitions (expression + descending flag).
    /// Multiple orderings are applied in sequence.
    /// </summary>
    IReadOnlyList<(Expression<Func<T, object>> Key, bool Descending)> OrderBy { get; }

    /// <summary>
    /// Number of records to skip (for paging).
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Number of records to take (for paging).
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Whether the query should be executed as read-only (no tracking).
    /// Only relevant for providers that support change tracking (e.g., EF).
    /// </summary>
    bool AsNoTracking { get; }
}
