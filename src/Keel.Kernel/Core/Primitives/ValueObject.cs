namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Base class for DDD value objects with structural equality and stable hashing.
/// Derive from this type and implement <see cref="GetEqualityComponents"/>.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns the components that participate in equality.
    /// The sequence order matters and must be stable.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj) =>
        obj is ValueObject other && Equals(other);

    public bool Equals(ValueObject? other) =>
        other is not null &&
        GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var component in GetEqualityComponents())
                hash = (hash * 31) + (component?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public static bool operator ==(ValueObject? a, ValueObject? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
}
