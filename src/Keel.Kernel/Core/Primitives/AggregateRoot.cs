using Keel.Kernel.Core.DomainEvents;

namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Base aggregate root that supports collecting domain events.
/// </summary>
/// <typeparam name="TKey">Stable, comparable id type.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot, IHasDomainEvents
    where TKey : IEquatable<TKey>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected AggregateRoot() { }
    protected AggregateRoot(TKey id) : base(id) { }

    /// <summary>Read-only view of events raised during this UoW.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Registers a new domain event to be dispatched later.</summary>
    protected void Raise(IDomainEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        _domainEvents.Add(@event);
    }

    /// <summary>Clears all accumulated events (typically after dispatch).</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
