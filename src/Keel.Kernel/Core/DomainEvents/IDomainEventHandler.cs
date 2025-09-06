namespace Keel.Kernel.Core.DomainEvents;

/// <summary>
/// Handles a specific domain event type in-process.
/// Keep handlers idempotent; dispatcher may retry in some setups.
/// </summary>
/// <typeparam name="TEvent">Domain event type.</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>Handle the given domain event.</summary>
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
