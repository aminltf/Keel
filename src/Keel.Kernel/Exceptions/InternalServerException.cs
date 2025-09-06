namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for unexpected, unrecoverable errors in the system.
/// Typically mapped to HTTP 500.
/// Use for generic catch-all logging; not for expected flows.
/// </summary>
public sealed class InternalServerException : DomainException
{
    public InternalServerException(string message = "An unexpected error occurred.")
        : base("System.Internal", message)
    {
    }

    public InternalServerException(string message, Exception innerException)
        : base("System.Internal", message, innerException)
    {
    }
}
