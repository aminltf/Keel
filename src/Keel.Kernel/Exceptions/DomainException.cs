using Keel.Kernel.Core.Primitives;

namespace Keel.Kernel.Exceptions;

/// <summary>
/// Base exception type for domain-related errors.
/// Prefer using <see cref="Error"/> and <see cref="Result"/> for expected flows,
/// but throw DomainException (or derived) for critical domain invariant violations.
/// </summary>
public class DomainException : Exception
{
    /// <summary>Stable machine-readable error code.</summary>
    public string Code { get; }

    /// <summary>Optional additional details (key-value bag).</summary>
    public IReadOnlyDictionary<string, object?> Details { get; }

    public DomainException(string code, string message)
        : base(message)
    {
        Code = code;
        Details = new Dictionary<string, object?>();
    }

    public DomainException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
        Details = new Dictionary<string, object?>();
    }

    public DomainException(string code, string message, IDictionary<string, object?> details)
        : base(message)
    {
        Code = code;
        Details = new Dictionary<string, object?>(details);
    }

    public override string ToString() =>
        $"{GetType().Name}: [{Code}] {Message}";
}
