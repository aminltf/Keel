namespace Keel.Kernel.Core.DomainEvents;

/// <summary>
/// Aggregates that produce domain events should expose them via this contract
/// so they can be collected and dispatched by the application/infrastructure layer.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>Uncommitted events raised during the current unit of work.</summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>Clears events after they are dispatched.</summary>
    void ClearDomainEvents();
}
