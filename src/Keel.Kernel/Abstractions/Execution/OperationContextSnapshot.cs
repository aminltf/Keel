using Keel.Kernel.Abstractions.Time;
using Keel.Kernel.Abstractions.Identity;
using Keel.Kernel.Abstractions.Correlation;

namespace Keel.Kernel.Abstractions.Execution;

/// <summary>
/// A simple, immutable snapshot implementation of <see cref="IOperationContext{TUserKey, TTenantId}"/>.
/// Handy for tests, background jobs, and passing a captured context into pipelines.
/// </summary>
public sealed class OperationContextSnapshot<TUserKey, TTenantId> : IOperationContext<TUserKey, TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    public IClock Clock { get; }
    public ICurrentUser<TUserKey> CurrentUser { get; }
    public ICorrelationIdAccessor Correlation { get; }
    public TTenantId? TenantId { get; }
    public IReadOnlyDictionary<string, object?> Items { get; }

    public OperationContextSnapshot(
        IClock clock,
        ICurrentUser<TUserKey> currentUser,
        ICorrelationIdAccessor correlation,
        TTenantId? tenantId = default,
        IReadOnlyDictionary<string, object?>? items = null)
    {
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        Correlation = correlation ?? throw new ArgumentNullException(nameof(correlation));
        TenantId = tenantId;
        Items = items ?? new Dictionary<string, object?>();
    }
}
