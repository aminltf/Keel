namespace Keel.Kernel.Exceptions;

/// <summary>
/// Exception for violated domain business rules (aggregates invariants).
/// Use when a business rule must be enforced strongly with exception semantics.
/// </summary>
public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException(string code, string message)
        : base(code, message)
    {
    }
}
