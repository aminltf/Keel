namespace Keel.Kernel.Abstractions.Domain;

/// <summary>
/// Multitenancy boundary marker. Attach this to entities/aggregates that belong to a tenant.
/// Enables per-tenant filters, indexes, and authorization checks.
/// </summary>
/// <typeparam name="TTenantId">Identifier type for the tenant (e.g., <see cref="Guid"/>, <see cref="long"/>, <see cref="string"/>).</typeparam>
public interface ITenantScoped<TTenantId> where TTenantId : IEquatable<TTenantId>
{
    /// <summary>Tenant identifier for this record.</summary>
    TTenantId TenantId { get; set; }
}
