namespace Keel.Kernel.Abstractions.Correlation;

/// <summary>
/// Provides access to the correlation identifier associated with the current logical operation/request.
/// Infrastructure (e.g., Web/API) is responsible for setting it (from headers or generated).
/// </summary>
public interface ICorrelationIdAccessor
{
    /// <summary>
    /// The correlation identifier flowing across service boundaries (nullable if not set).
    /// Prefer a stable, opaque string (e.g., Guid "N" format).
    /// </summary>
    string? CorrelationId { get; }
}
