using System.Security.Claims;

namespace Keel.Web.Execution;

/// <summary>
/// Options controlling how the OperationContext is populated from HTTP.
/// </summary>
public sealed class OperationContextOptions<TUserKey, TTenantId>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    /// <summary>Header name for correlation id. Default: "X-Correlation-Id".</summary>
    public string CorrelationHeader { get; set; } = "X-Correlation-Id";

    /// <summary>Header name for tenant id. Default: "X-Tenant-Id".</summary>
    public string TenantHeader { get; set; } = "X-Tenant-Id";

    // Claim types
    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;
    public string UserNameClaimType { get; set; } = ClaimTypes.Name;
    public string EmailClaimType { get; set; } = ClaimTypes.Email;
    public string RoleClaimType { get; set; } = ClaimTypes.Role;

    /// <summary>Parser for converting claim string to TUserKey (required).</summary>
    public Func<string, TUserKey?> ParseUserId { get; set; } = _ => default;

    /// <summary>Parser for converting header string to TTenantId (optional for non-tenant systems).</summary>
    public Func<string, TTenantId?> ParseTenantId { get; set; } = _ => default;

    /// <summary>Generate a new correlation id when not provided by the client.</summary>
    public Func<string> GenerateCorrelationId { get; set; } = () => Guid.NewGuid().ToString("N");
}
