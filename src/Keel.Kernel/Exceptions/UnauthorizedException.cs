namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for unauthenticated access attempts (no/invalid credentials).
/// Typically mapped to HTTP 401.
/// </summary>
public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "User is not authenticated.")
        : base("Auth.Unauthorized", message)
    {
    }
}
