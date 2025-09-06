namespace Keel.Kernel.Abstractions.Identity;

/// <summary>
/// Provides information about the current authenticated user in a transport-agnostic way
/// (no dependency on ASP.NET Core, Claims, or any specific identity provider).
/// The user key type is generic to accommodate different identity systems.
/// </summary>
/// <typeparam name="TUserKey">
/// The identifier type for users (e.g., <see cref="Guid"/>, <see cref="long"/>, <see cref="string"/>).
/// Must be comparable for reliable equality checks.
/// </typeparam>
public interface ICurrentUser<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// True if the request is associated with an authenticated principal.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Identifier of the current user; null if unauthenticated or anonymous.
    /// </summary>
    TUserKey? UserId { get; }

    /// <summary>
    /// Display name or username if available; otherwise null.
    /// Keep this optional to avoid coupling with a specific identity schema.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Email address if available; otherwise null.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Collection of role names associated with the current user; empty if none.
    /// Use for coarse-grained authorization hints in upper layers (not in domain).
    /// </summary>
    IEnumerable<string> Roles { get; }
}
