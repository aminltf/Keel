namespace Keel.Kernel.Core.DomainEvents;

/// <summary>
/// Abstraction for dispatching domain events produced by aggregates.
/// Implement in the Application/Infrastructure layer (e.g., in-proc handlers, MediatR bridge, message bus).
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a collection of domain events asynchronously.
    /// Implementations should guarantee at-least-once semantics within the unit of work boundary.
    /// </summary>
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}
