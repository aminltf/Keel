namespace Keel.Kernel.Abstractions.Domain;

/// <summary>
/// Opt-in contract for optimistic concurrency control using a database-generated
/// version token. This token must change whenever the row changes.
/// 
/// Notes:
/// - Keep the token opaque to the application (do not parse/modify).
/// - Infrastructure (e.g., EF Core mapping) should mark this property as a
///   concurrency token and configure database generation.
/// - Typical SQL Server mapping: rowversion / timestamp -> byte[] in C#.
/// </summary>
public interface IHasRowVersion
{
    /// <summary>
    /// Database-generated version token. Replace on every update by the data store.
    /// Commonly mapped to SQL Server 'rowversion' and used by the ORM to detect
    /// write conflicts (lost updates).
    /// </summary>
    byte[] RowVersion { get; set; }
}
