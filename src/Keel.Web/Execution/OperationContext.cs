using Keel.Kernel.Abstractions.Correlation;
using Keel.Kernel.Abstractions.Execution;
using Keel.Kernel.Abstractions.Identity;
using Keel.Kernel.Abstractions.Time;

namespace Keel.Web.Execution;

/// <summary>
/// Concrete web snapshot of IOperationContext{TUserKey, TTenantId}.
/// </summary>
public sealed class OperationContext<TUserKey, TTenantId> : IOperationContext<TUserKey, TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    public IClock Clock { get; }
    public ICurrentUser<TUserKey> CurrentUser { get; }
    public ICorrelationIdAccessor Correlation { get; }
    public TTenantId? TenantId { get; }
    public IReadOnlyDictionary<string, object?> Items { get; }

    public OperationContext(
        IClock clock,
        ICurrentUser<TUserKey> currentUser,
        ICorrelationIdAccessor correlation,
        TTenantId? tenantId,
        IReadOnlyDictionary<string, object?>? items = null)
    {
        Clock = clock;
        CurrentUser = currentUser;
        Correlation = correlation;
        TenantId = tenantId;
        Items = items ?? new Dictionary<string, object?>();
    }
}
