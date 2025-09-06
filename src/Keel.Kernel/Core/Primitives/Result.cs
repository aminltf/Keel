namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// Use Failure with a domain-specific <see cref="Error"/> instead of throwing exceptions
/// for expected control-flow (validation, not-found, business rule violations).
/// </summary>
public readonly struct Result
{
    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Optional error explaining the failure cause when <see cref="IsSuccess"/> is false.</summary>
    public Error Error { get; }

    private Result(bool success, Error error)
    {
        IsSuccess = success;
        Error = error;
    }

    /// <summary>Create a successful result.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>Create a failed result with a specific error code and message.</summary>
    public static Result Failure(string code, string message) => new(false, new Error(code, message));

    /// <summary>Create a failed result with an <see cref="Error"/> instance.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Convenience conversion from <see cref="Error"/> to a failed <see cref="Result"/>.</summary>
    public static implicit operator Result(Error error) => Failure(error);

    /// <summary>Throw if the result is failure; useful at application boundaries.</summary>
    public void EnsureSuccess()
    {
        if (!IsSuccess) throw new InvalidOperationException($"Operation failed: {Error.Code} - {Error.Message}");
    }

    /// <summary>Monadic bind for result-chaining with no return value.</summary>
    public Result Then(Func<Result> next) => IsSuccess ? next() : this;
}

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
/// <typeparam name="T">Returned value type when the operation succeeds.</typeparam>
public readonly struct Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error Error { get; }

    private Result(bool success, T? value, Error error)
    {
        IsSuccess = success;
        Value = value;
        Error = error;
    }

    /// <summary>Create a successful result with a value.</summary>
    public static Result<T> Success(T value) => new(true, value, Error.None);

    /// <summary>Create a failed result with an error.</summary>
    public static Result<T> Failure(Error error) => new(false, default, error);

    /// <summary>Convenience conversion from value to success result.</summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>Convenience conversion from error to failed result.</summary>
    public static implicit operator Result<T>(Error error) => Failure(error);

    /// <summary>Map the success value to another type; propagate failure as-is.</summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper) =>
        IsSuccess ? Result<TResult>.Success(mapper(Value!)) : Result<TResult>.Failure(Error);

    /// <summary>Bind another operation that returns a Result based on the success value.</summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder) =>
        IsSuccess ? binder(Value!) : Result<TResult>.Failure(Error);

    /// <summary>Throw if the result is failure; return the value otherwise.</summary>
    public T EnsureSuccess()
    {
        if (!IsSuccess) throw new InvalidOperationException($"Operation failed: {Error.Code} - {Error.Message}");
        return Value!;
    }
}
