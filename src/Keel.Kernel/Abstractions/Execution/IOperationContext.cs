using Keel.Kernel.Abstractions.Time;
using Keel.Kernel.Abstractions.Identity;
using Keel.Kernel.Abstractions.Correlation;

namespace Keel.Kernel.Abstractions.Execution;

/// <summary>
/// A lightweight execution context that exposes cross-cutting concerns commonly needed
/// by application/domain services: time, current user, correlation id, and tenant boundary.
/// </summary>
/// <typeparam name="TUserKey">Identifier type for users (e.g., Guid/long/string).</typeparam>
/// <typeparam name="TTenantId">Identifier type for tenants (e.g., Guid/long/string).</typeparam>
public interface IOperationContext<TUserKey, TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    /// <summary>Time provider (always UTC).</summary>
    IClock Clock { get; }

    /// <summary>Abstraction over the authenticated principal.</summary>
    ICurrentUser<TUserKey> CurrentUser { get; }

    /// <summary>Access to the ambient correlation identifier (for logs/tracing).</summary>
    ICorrelationIdAccessor Correlation { get; }

    /// <summary>Current tenant identifier, if the request is tenant-scoped; otherwise null.</summary>
    TTenantId? TenantId { get; }

    /// <summary>
    /// A small, transport-agnostic property bag for passing local, request-scoped items.
    /// Keep it minimal to avoid hidden dependencies.
    /// </summary>
    IReadOnlyDictionary<string, object?> Items { get; }
}
