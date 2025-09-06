namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Optional container to represent the presence or absence of a value
/// without resorting to null semantics everywhere.
/// </summary>
/// <typeparam name="T">The wrapped value type.</typeparam>
public readonly struct Maybe<T>
{
    /// <summary>The contained value (may be null if HasValue is false).</summary>
    public T? Value { get; }

    /// <summary>True if a value is present.</summary>
    public bool HasValue { get; }

    private Maybe(T? value, bool hasValue)
    {
        Value = value;
        HasValue = hasValue;
    }

    /// <summary>Create a Maybe with a value.</summary>
    public static Maybe<T> Some(T value) => new(value, hasValue: true);

    /// <summary>Create an empty Maybe.</summary>
    public static Maybe<T> None() => new(default, hasValue: false);

    /// <summary>Implicit conversion from value to Maybe (Some if not null, else None).</summary>
    public static implicit operator Maybe<T>(T? value) =>
        value is null ? None() : Some(value);

    /// <summary>Map the contained value if present; otherwise propagate None.</summary>
    public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) =>
        HasValue ? Maybe<TResult>.Some(mapper(Value!)) : Maybe<TResult>.None();

    /// <summary>Bind to another Maybe-producing function if present.</summary>
    public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder) =>
        HasValue ? binder(Value!) : Maybe<TResult>.None();
}
