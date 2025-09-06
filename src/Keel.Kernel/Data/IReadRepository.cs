using Keel.Kernel.Core.Querying;

namespace Keel.Kernel.Data;

/// <summary>
/// Read-only repository abstraction for aggregate roots or entities.
/// Works with specifications to decouple query logic from data providers.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TKey">Entity key type.</typeparam>
public interface IReadRepository<TEntity, in TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    /// <summary>Gets an entity by its identifier, or null if not found.</summary>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);

    /// <summary>Gets the first entity matching a specification, or null if none.</summary>
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken ct = default);

    /// <summary>Gets all entities matching a specification.</summary>
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken ct = default);

    /// <summary>Counts entities matching a specification.</summary>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default);

    /// <summary>Checks existence of any entity matching a specification.</summary>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
}
