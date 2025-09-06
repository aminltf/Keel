using Microsoft.AspNetCore.Http;
using Keel.Kernel.Abstractions.Execution;
using Keel.Kernel.Abstractions.Identity;
using Keel.Kernel.Abstractions.Time;
using Keel.Web.Tenancy;
using Keel.Web.Execution;
using Keel.Kernel.Abstractions.Correlation;

namespace Keel.Web.Middleware;

/// <summary>
/// Builds and pushes an OperationContext for the lifetime of the HTTP request.
/// </summary>
public sealed class OperationContextMiddleware<TUserKey, TTenantId> : IMiddleware
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    private readonly IClock _clock;
    private readonly ICurrentUser<TUserKey> _currentUser;
    private readonly ICorrelationIdAccessor _correlation;
    private readonly ITenantResolver<TTenantId> _tenantResolver;
    private readonly IOperationContextAccessor<TUserKey, TTenantId> _accessor;

    public OperationContextMiddleware(
        IClock clock,
        ICurrentUser<TUserKey> currentUser,
        ICorrelationIdAccessor correlation,
        ITenantResolver<TTenantId> tenantResolver,
        IOperationContextAccessor<TUserKey, TTenantId> accessor)
    {
        _clock = clock;
        _currentUser = currentUser;
        _correlation = correlation;
        _tenantResolver = tenantResolver;
        _accessor = accessor;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _tenantResolver.TryResolve(context, out var tenantId);

        var ctx = new OperationContext<TUserKey, TTenantId>(
            _clock, _currentUser, _correlation, tenantId);

        using (_accessor.Push(ctx))
        {
            await next(context);
        }
    }
}
