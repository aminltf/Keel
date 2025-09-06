namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for unauthorized access to a resource (authenticated but no permission).
/// Typically mapped to HTTP 403.
/// </summary>
public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "User is not authorized to perform this action.")
        : base("Auth.Forbidden", message)
    {
    }
}
