using Keel.Kernel.Core.Primitives;

namespace Keel.Kernel.Validation;

/// <summary>
/// Aggregates validation errors. Use alongside Result/Error for transport.
/// </summary>
public sealed class ValidationResult
{
    private static readonly IReadOnlyList<ValidationError> _empty = Array.Empty<ValidationError>();

    public bool IsValid => Errors.Count == 0;

    /// <summary>Collection of validation failures.</summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    private ValidationResult(IReadOnlyList<ValidationError> errors) =>
        Errors = errors ?? _empty;

    /// <summary>Create a valid result (no errors).</summary>
    public static ValidationResult Success() => new(_empty);

    /// <summary>Create an invalid result with one or more errors.</summary>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) =>
        new((errors ?? Enumerable.Empty<ValidationError>()).ToArray());

    /// <summary>Merges multiple ValidationResults into one.</summary>
    public static ValidationResult Merge(params ValidationResult[] results) =>
        Failure(results.SelectMany(r => r.Errors));

    /// <summary>
    /// Converts this validation result to a domain <see cref="Result"/>.
    /// Code format: "Validation.Failed". Errors are attached via <see cref="Error.Message"/>.
    /// </summary>
    public Result ToResult()
    {
        if (IsValid) return Result.Success();

        // Compact human-friendly message; details بهتر است در لایه‌ی وب به ProblemDetails.Extensions منتقل شود.
        var message = Errors.Count == 1
            ? Errors[0].Message
            : $"Validation failed with {Errors.Count} error(s).";

        return Result.Failure("Validation.Failed", message);
    }

    /// <summary>
    /// Converts this validation result to a typed <see cref="Result{T}"/>.
    /// </summary>
    public Result<T> ToResult<T>() =>
        IsValid ? Result<T>.Failure(Error.None) // shouldn't happen; caller should provide value
                : Result<T>.Failure(new Error("Validation.Failed",
                    Errors.Count == 1 ? Errors[0].Message : $"Validation failed with {Errors.Count} error(s)."));
}
