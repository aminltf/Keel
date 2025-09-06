using Keel.Kernel.Exceptions;

namespace Keel.Kernel.Abstractions.Execution;

/// <summary>
/// Convenience helpers for common context-based checks and values.
/// </summary>
public static class OperationContextExtensions
{
    /// <summary>Shortcut to the current UTC time.</summary>
    public static DateTimeOffset UtcNow<TUserKey, TTenantId>(
        this IOperationContext<TUserKey, TTenantId> ctx)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
        => ctx.Clock.UtcNow;

    /// <summary>True if there is an authenticated principal.</summary>
    public static bool IsAuthenticated<TUserKey, TTenantId>(
        this IOperationContext<TUserKey, TTenantId> ctx)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
        => ctx.CurrentUser.IsAuthenticated;

    /// <summary>
    /// Returns the current user's identifier or throws <see cref="UnauthorizedException"/> if unauthenticated.
    /// </summary>
    public static TUserKey RequireUserId<TUserKey, TTenantId>(
        this IOperationContext<TUserKey, TTenantId> ctx)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
    {
        if (!ctx.CurrentUser.IsAuthenticated || ctx.CurrentUser.UserId is null)
            throw new UnauthorizedException("User is not authenticated.");
        return ctx.CurrentUser.UserId;
    }

    /// <summary>
    /// Returns the current tenant identifier or throws a <see cref="ValidationException"/> if not present.
    /// </summary>
    public static TTenantId RequireTenantId<TUserKey, TTenantId>(
        this IOperationContext<TUserKey, TTenantId> ctx)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
    {
        if (ctx.TenantId is null)
            throw new ValidationException("Tenant.Missing", "Tenant id is required for this operation.");
        return ctx.TenantId;
    }

    /// <summary>Retrieves a typed item from the context property bag if present; otherwise default.</summary>
    public static T? GetItem<TUserKey, TTenantId, T>(
        this IOperationContext<TUserKey, TTenantId> ctx, string key)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
    {
        if (ctx.Items.TryGetValue(key, out var o) && o is T typed) return typed;
        return default;
    }
}
