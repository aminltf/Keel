namespace Keel.Kernel.Abstractions.Domain;

/// <summary>
/// Creation and last-modification audit metadata (UTC) for domain objects.
/// This contract is generic over the user key type to fit different identity systems.
/// Typically assigned by infrastructure (e.g., SaveChanges interceptor).
/// </summary>
/// <typeparam name="TUserKey">
/// The identifier type for users (e.g., <see cref="Guid"/>, <see cref="long"/>, <see cref="string"/>).
/// Must be comparable to ensure reliable equality checks.
/// </typeparam>
public interface IAuditable<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    /// <summary>Creator identity (user id/username/email). Null for system-generated records.</summary>
    TUserKey? CreatedBy { get; set; }

    /// <summary>Creation timestamp in UTC.</summary>
    DateTimeOffset CreatedOnUtc { get; set; }

    /// <summary>Last modifier identity; null if never modified.</summary>
    TUserKey? LastModifiedBy { get; set; }

    /// <summary>Last modification timestamp in UTC; null if never modified.</summary>
    DateTimeOffset? LastModifiedOnUtc { get; set; }
}
