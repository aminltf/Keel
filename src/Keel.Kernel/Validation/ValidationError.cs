namespace Keel.Kernel.Validation;

/// <summary>
/// Represents a single validation failure for a specific field or rule.
/// </summary>
public readonly record struct ValidationError(
    string Code,
    string Message,
    string? Field = null);
