namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for domain validation errors (business rule violations).
/// Unlike System.ComponentModel.DataAnnotations.ValidationException,
/// this one is tailored for DDD-style aggregates.
/// </summary>
public sealed class ValidationException : DomainException
{
    public ValidationException(string code, string message)
        : base(code, message)
    {
    }

    public ValidationException(string code, string message, IDictionary<string, object?> details)
        : base(code, message, details)
    {
    }
}
