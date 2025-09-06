using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Keel.Web.Execution;

namespace Keel.Web.Tenancy;

/// <summary>
/// Resolves tenant id from a configured request header (e.g., "X-Tenant-Id").
/// </summary>
public sealed class HeaderTenantResolver<TUserKey, TTenantId> : ITenantResolver<TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    private readonly OperationContextOptions<TUserKey, TTenantId> _opts;

    public HeaderTenantResolver(IOptions<OperationContextOptions<TUserKey, TTenantId>> opts)
        => _opts = opts.Value;

    public bool TryResolve(HttpContext http, out TTenantId? tenantId)
    {
        tenantId = default;
        if (!http.Request.Headers.TryGetValue(_opts.TenantHeader, out var val))
            return false;

        var s = val.ToString();
        if (string.IsNullOrWhiteSpace(s)) return false;

        tenantId = _opts.ParseTenantId(s);
        return tenantId is not null && !tenantId.Equals(default);
    }
}
