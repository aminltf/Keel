namespace Keel.Kernel.Validation;

/// <summary>
/// Minimal contract for validating a value or request object.
/// Implementations can wrap FluentValidation or any custom rule engine.
/// </summary>
/// <typeparam name="T">Validated object type.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the provided instance and returns a <see cref="ValidationResult"/>.
    /// Should not throw for expected validation failures.
    /// </summary>
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken ct = default);
}
