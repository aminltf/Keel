namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Defensive programming helpers for validating arguments and invariants.
/// Prefer using these in constructors and application service entry-points
/// to fail fast with clear messages.
/// </summary>
public static class Guard
{
    /// <summary>Throws <see cref="ArgumentNullException"/> if value is null.</summary>
    public static T NotNull<T>(T? value, string paramName) where T : class =>
        value ?? throw new ArgumentNullException(paramName);

    /// <summary>Throws <see cref="ArgumentException"/> if string is null/empty/whitespace.</summary>
    public static string NotNullOrWhiteSpace(string? value, string paramName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{paramName} must not be empty.", paramName)
            : value!;

    /// <summary>Throws <see cref="ArgumentOutOfRangeException"/> if value &lt; min or &gt; max.</summary>
    public static int InRange(int value, int min, int max, string paramName) =>
        value < min || value > max
            ? throw new ArgumentOutOfRangeException(paramName, value, $"Must be in range [{min}..{max}].")
            : value;

    /// <summary>Throws <see cref="ArgumentOutOfRangeException"/> if value is not strictly positive.</summary>
    public static int Positive(int value, string paramName) =>
        value <= 0
            ? throw new ArgumentOutOfRangeException(paramName, value, "Must be positive.")
            : value;

    /// <summary>Throws <see cref="ArgumentOutOfRangeException"/> if value is negative.</summary>
    public static int NonNegative(int value, string paramName) =>
        value < 0
            ? throw new ArgumentOutOfRangeException(paramName, value, "Must be non-negative.")
            : value;
}
