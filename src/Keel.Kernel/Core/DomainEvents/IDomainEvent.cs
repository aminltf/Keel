namespace Keel.Kernel.Core.DomainEvents;

/// <summary>
/// Marker contract for domain events. Domain events represent something
/// that has happened in the domain and may trigger side effects.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Unique id for event de-duplication and tracing.</summary>
    Guid EventId { get; }

    /// <summary>Occurrence time in UTC (set at creation).</summary>
    DateTimeOffset OccurredOnUtc { get; }
}
