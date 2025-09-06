namespace Keel.Kernel.Messaging.Outbox;

/// <summary>
/// State of an outbox message in a transactional outbox pattern.
/// </summary>
public enum OutboxMessageState
{
    Pending = 0, // Stored but not picked up by the dispatcher
    Processing = 1, // Being processed by a worker/dispatcher
    Succeeded = 2, // Successfully dispatched to the broker/handler
    Failed = 3  // Exhausted retries; requires manual attention or parked
}
