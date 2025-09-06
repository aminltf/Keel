namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for missing domain entities/resources.
/// Typically mapped to HTTP 404 in web applications.
/// </summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string entityName, string id)
        : base($"{entityName}.NotFound", $"{entityName} with Id '{id}' was not found.")
    {
    }
}
