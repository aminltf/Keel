namespace Keel.Kernel.Abstractions.Execution;

/// <summary>
/// Accessor to the current <see cref="IOperationContext{TUserKey, TTenantId}"/>.
/// Implementations (e.g., Web, Workers) are responsible for setting/clearing the ambient context
/// at the boundary of a request or background job.
/// </summary>
public interface IOperationContextAccessor<TUserKey, TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    /// <summary>The current operation context (null if not assigned).</summary>
    IOperationContext<TUserKey, TTenantId>? Current { get; set; }

    /// <summary>
    /// Pushes the given context as the current one and returns a disposable that restores the previous value.
    /// Useful for scoped overrides in tests or background flows.
    /// </summary>
    IDisposable Push(IOperationContext<TUserKey, TTenantId> context);
}
