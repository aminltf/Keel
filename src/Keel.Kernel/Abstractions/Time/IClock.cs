namespace Keel.Kernel.Abstractions.Time;

/// <summary>
/// Abstraction over system time to make time-dependent logic testable and consistent.
/// Always represent time in UTC to avoid timezone ambiguities.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Current moment in UTC. Implementations should avoid heavy operations
    /// and return a cheap, accurate timestamp.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
