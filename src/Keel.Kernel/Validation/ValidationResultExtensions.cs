using Keel.Kernel.Core.Primitives;

namespace Keel.Kernel.Validation;

public static class ValidationResultExtensions
{
    /// <summary>
    /// Converts a valid result into <see cref="Result.Success"/>, or returns a failure Result with a standard code.
    /// </summary>
    public static Result ToFailureIfInvalid(this ValidationResult vr) =>
        vr.IsValid ? Result.Success()
                   : Result.Failure("Validation.Failed",
                        vr.Errors.Count == 1 ? vr.Errors[0].Message : $"Validation failed with {vr.Errors.Count} error(s).");

    /// <summary>
    /// Converts a valid result into a successful <see cref="Result{T}"/> with the provided value; otherwise a failure result.
    /// </summary>
    public static Result<T> ToFailureIfInvalid<T>(this ValidationResult vr, T valueIfValid) =>
        vr.IsValid ? Result<T>.Success(valueIfValid)
                   : Result<T>.Failure(new Error("Validation.Failed",
                        vr.Errors.Count == 1 ? vr.Errors[0].Message : $"Validation failed with {vr.Errors.Count} error(s)."));
}
