namespace Keel.Kernel.Messaging.Outbox;

/// <summary>
/// Abstraction over the outbox persistence. Implement in Infrastructure (e.g., EF Core, Dapper).
/// </summary>
public interface IOutboxStore
{
    /// <summary>
    /// Persists a new outbox message in Pending state within the caller's unit of work/transaction.
    /// </summary>
    Task EnqueueAsync(
        string type,
        string payload,
        DateTimeOffset occurredOnUtc,
        string? traceId = null,
        string? correlationId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Fetches a batch of pending messages for processing and marks them as Processing atomically.
    /// The implementation MUST ensure that messages are not picked by multiple workers concurrently.
    /// </summary>
    Task<IReadOnlyList<IOutboxMessage>> DequeueForProcessingAsync(
        int batchSize,
        CancellationToken ct = default);

    /// <summary>Marks a message as successfully dispatched.</summary>
    Task MarkSucceededAsync(Guid id, CancellationToken ct = default);

    /// <summary>Marks a message as failed and increments Attempt; optionally store a short error.</summary>
    Task MarkFailedAsync(Guid id, string? error = null, CancellationToken ct = default);
}
