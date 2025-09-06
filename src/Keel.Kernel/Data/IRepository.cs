namespace Keel.Kernel.Data;

/// <summary>
/// Full repository abstraction (read + write + common utility methods).
/// This design is persistence-agnostic; infrastructure can implement using EF, Dapper, Mongo, etc.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TKey">Entity key type.</typeparam>
public interface IRepository<TEntity, in TKey> : IReadRepository<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    // Create
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    // Update
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    // Delete
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    // Upsert
    Task AddOrUpdateAsync(TEntity entity, CancellationToken ct = default);
    Task AddOrUpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    // Utilities
    /// <summary>
    /// Loads the latest state of an entity from the underlying store,
    /// discarding any in-memory modifications (if supported by provider).
    /// </summary>
    Task ReloadAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Returns entities matching a list of ids (efficient batch fetch).
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, CancellationToken ct = default);

    /// <summary>
    /// Attaches an entity to the current context/session without marking it modified.
    /// Useful for disconnected scenarios (e.g., web API updates).
    /// </summary>
    void Attach(TEntity entity);

    /// <summary>
    /// Detaches an entity from the current context/session, stopping change tracking.
    /// Useful for reducing memory footprint or explicit lifecycle control.
    /// </summary>
    void Detach(TEntity entity);

    /// <summary>
    /// Marks the entity as tracked (if supported).
    /// </summary>
    void Track(TEntity entity);

    /// <summary>
    /// Marks the entity as untracked (if supported).
    /// </summary>
    void Untrack(TEntity entity);
}
