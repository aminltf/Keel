namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for optimistic concurrency violations.
/// Typically mapped to HTTP 409 in web applications.
/// </summary>
public sealed class ConcurrencyException : DomainException
{
    public ConcurrencyException(string entityName, string id)
        : base($"{entityName}.Concurrency", $"Concurrency conflict on {entityName} with Id '{id}'.")
    {
    }
}
