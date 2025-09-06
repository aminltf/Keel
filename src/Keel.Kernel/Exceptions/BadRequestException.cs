namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for invalid client input (syntax or semantic errors).
/// Typically mapped to HTTP 400.
/// </summary>
public sealed class BadRequestException : DomainException
{
    public BadRequestException(string message = "The request is invalid.")
        : base("Request.Bad", message)
    {
    }
}
