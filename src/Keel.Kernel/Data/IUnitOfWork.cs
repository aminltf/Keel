namespace Keel.Kernel.Data;

/// <summary>
/// Unit of Work abstraction. Coordinates repositories and transactions.
/// Keeps business logic free of persistence details.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Persists all tracked changes to the store.
    /// </summary>
    Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess = true,
        CancellationToken ct = default);

    /// <summary>
    /// Starts an explicit transaction (if supported by provider).
    /// </summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Executes a function inside a transaction boundary.
    /// Ensures commit on success, rollback on failure.
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default);

    /// <summary>
    /// Whether change tracking is enabled (provider-dependent).
    /// Can be used to disable tracking for bulk-read scenarios.
    /// </summary>
    bool TrackChanges { get; set; }
}
