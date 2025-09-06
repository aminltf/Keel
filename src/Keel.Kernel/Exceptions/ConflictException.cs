namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for state conflicts that are not concurrency tokens (e.g., duplicate unique fields).
/// Typically mapped to HTTP 409.
/// </summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message = "A conflict occurred with the current state of the resource.")
        : base("Resource.Conflict", message)
    {
    }
}
