using System.Threading;
using Keel.Kernel.Abstractions.Correlation;

namespace Keel.Web.Execution.Correlation;

/// <summary>
/// AsyncLocal-based correlation id accessor for web scenarios.
/// </summary>
public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private static readonly AsyncLocal<string?> _current = new();

    public string? CorrelationId => _current.Value;

    internal void Set(string? id) => _current.Value = id;

    internal void Clear() => _current.Value = null;
}
