using Keel.Kernel.Abstractions.Execution;

namespace Keel.Web.Execution;

/// <summary>
/// AsyncLocal-based accessor for OperationContext.
/// </summary>
public sealed class OperationContextAccessor<TUserKey, TTenantId> : IOperationContextAccessor<TUserKey, TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    private static readonly AsyncLocal<IOperationContext<TUserKey, TTenantId>?> _current = new();

    public IOperationContext<TUserKey, TTenantId>? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    public IDisposable Push(IOperationContext<TUserKey, TTenantId> context)
    {
        var prev = _current.Value;
        _current.Value = context;
        return new PopDisposable(() => _current.Value = prev);
    }

    private sealed class PopDisposable : IDisposable
    {
        private readonly Action _pop;
        public PopDisposable(Action pop) => _pop = pop;
        public void Dispose() => _pop();
    }
}
