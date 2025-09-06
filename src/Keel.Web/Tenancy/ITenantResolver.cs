using Microsoft.AspNetCore.Http;

namespace Keel.Web.Tenancy;

/// <summary>
/// Resolves tenant identifier from the HTTP request.
/// </summary>
public interface ITenantResolver<TTenantId> where TTenantId : IEquatable<TTenantId>
{
    bool TryResolve(HttpContext http, out TTenantId? tenantId);
}
