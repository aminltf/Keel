using System.Linq.Expressions;

namespace Keel.Kernel.Core.Querying;

/// <summary>
/// Base class for building specifications fluently.
/// Extend this class to define query logic without tying to a specific ORM.
/// </summary>
/// <typeparam name="TEntity">The entity type being queried.</typeparam>
public abstract class Specification<TEntity> : ISpecification<TEntity>
{
    private readonly List<Expression<Func<TEntity, object>>> _includes = new();
    private readonly List<(Expression<Func<TEntity, object>>, bool)> _orderBy = new();

    public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }

    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => _includes;

    public IReadOnlyList<(Expression<Func<TEntity, object>> Key, bool Descending)> OrderBy => _orderBy;

    public int? Skip { get; protected set; }

    public int? Take { get; protected set; }

    public bool AsNoTracking { get; protected set; } = true;

    /// <summary>Adds a navigation property to include.</summary>
    public void AddInclude(Expression<Func<TEntity, object>> include) => _includes.Add(include);

    /// <summary>Orders ascending by the given key.</summary>
    public void AddOrderByAsc(Expression<Func<TEntity, object>> key) => _orderBy.Add((key, false));

    /// <summary>Orders descending by the given key.</summary>
    public void AddOrderByDesc(Expression<Func<TEntity, object>> key) => _orderBy.Add((key, true));

    /// <summary>Applies paging with skip/take values.</summary>
    public void SetPaging(int skip, int take) { Skip = skip; Take = take; }

    /// <summary>Sets the AsNoTracking flag (useful for read-only queries).</summary>
    public void WithTracking() => AsNoTracking = false;

    /// <summary>
    /// Adds (AND-composes) a predicate to the current criteria.
    /// If no criteria exist yet, sets it as the initial predicate.
    /// </summary>
    public void Where(Expression<Func<TEntity, bool>> predicate)
    {
        Criteria = Criteria is null ? predicate : Criteria.AndAlso(predicate);
    }

    // (Protected helpers kept for backward compatibility)
    protected void OrderByAsc(Expression<Func<TEntity, object>> key) => AddOrderByAsc(key);
    protected void OrderByDesc(Expression<Func<TEntity, object>> key) => AddOrderByDesc(key);
    protected void ApplyPaging(int skip, int take) => SetPaging(skip, take);
}
