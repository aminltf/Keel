using Keel.Kernel.Abstractions.Domain;

namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Base entity that carries a strongly-typed identifier and equality by identity.
/// </summary>
/// <typeparam name="TKey">Stable, comparable id type (e.g., <see cref="Guid"/>, <see cref="long"/>).</typeparam>
public abstract class Entity<TKey> : IHasId<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Unique identity. Keep setter protected to preserve invariants;
    /// prefer 'init' in concrete implementations.
    /// </summary>
    public TKey Id { get; protected init; } = default!;

    protected Entity() { }
    protected Entity(TKey id) => Id = id;

    public override bool Equals(object? obj) =>
        obj is Entity<TKey> other && EqualityComparer<TKey>.Default.Equals(Id, other.Id);

    public override int GetHashCode() => Id?.GetHashCode() ?? 0;

    public static bool operator ==(Entity<TKey>? a, Entity<TKey>? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(Entity<TKey>? a, Entity<TKey>? b) => !(a == b);
}
