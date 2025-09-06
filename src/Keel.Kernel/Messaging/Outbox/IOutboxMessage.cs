namespace Keel.Kernel.Messaging.Outbox;

/// <summary>
/// Minimal message contract used by the transactional outbox.
/// Store a serialized payload plus enough metadata for retries, tracing and auditing.
/// </summary>
public interface IOutboxMessage
{
    Guid Id { get; }

    /// <summary>Message type (e.g., fully-qualified .NET type name or logical event name).</summary>
    string Type { get; }

    /// <summary>Serialized payload (JSON or other agreed format).</summary>
    string Payload { get; }

    /// <summary>When the domain event happened (UTC).</summary>
    DateTimeOffset OccurredOnUtc { get; }

    /// <summary>Processing state.</summary>
    OutboxMessageState State { get; }

    /// <summary>Delivery attempt count.</summary>
    int Attempt { get; }

    /// <summary>Last processing timestamp in UTC (null if never processed).</summary>
    DateTimeOffset? ProcessedOnUtc { get; }

    /// <summary>Last error string if failed (keep short; detailed stack traces go to logs).</summary>
    string? Error { get; }

    /// <summary>Tracing identifiers for observability (optional).</summary>
    string? TraceId { get; }
    string? CorrelationId { get; }
}
