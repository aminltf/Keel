namespace Keel.Kernel.Abstractions.Domain;

/// <summary>
/// Standard soft-delete contract. Use with a global query filter in your data adapter.
/// Generic over user key type to keep identity concerns flexible.
/// </summary>
/// <typeparam name="TUserKey">
/// The identifier type for users (e.g., <see cref="Guid"/>, <see cref="long"/>, <see cref="string"/>).
/// Must be comparable to ensure reliable equality checks.
/// </typeparam>
public interface ISoftDeletable<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    /// <summary>Marks the row as logically deleted and excluded from default queries.</summary>
    bool IsDeleted { get; set; }

    /// <summary>Actor identity who performed the deletion; null if not deleted.</summary>
    TUserKey? DeletedBy { get; set; }

    /// <summary>Deletion timestamp in UTC; null if not deleted.</summary>
    DateTimeOffset? DeletedOnUtc { get; set; }

    /// <summary>Optional human-readable reason for deletion (useful for audits and reviews).</summary>
    string? DeletionReason { get; set; }
}
