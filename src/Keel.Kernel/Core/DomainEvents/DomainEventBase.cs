namespace Keel.Kernel.Core.DomainEvents;

/// <summary>
/// Convenience immutable base record for domain events.
/// Derive specific events from this record (init-only properties).
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOnUtc { get; init; } = DateTimeOffset.UtcNow;
}
