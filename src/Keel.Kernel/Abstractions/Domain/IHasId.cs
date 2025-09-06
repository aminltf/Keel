namespace Keel.Kernel.Abstractions.Domain;

/// <summary>
/// Contract for domain objects that expose a strongly-typed primary key.
/// The identifier is immutable at the contract level to prevent accidental changes.
/// </summary>
/// <typeparam name="TKey">
/// Stable and comparable type (e.g., <see cref="Guid"/>, <see cref="long"/>).
/// </typeparam>
public interface IHasId<out TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// The unique identifier of the entity. Keep it immutable in implementations
    /// (e.g., 'init' or a private setter) to preserve identity semantics.
    /// </summary>
    TKey Id { get; }
}
