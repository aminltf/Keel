namespace Keel.Kernel.Core.Primitives;

/// <summary>
/// Lightweight error descriptor used by Result/Result{T}.
/// Keep codes stable and machine-readable (e.g., "Entities.NotFound").
/// Messages are human-readable and may be localized at higher layers.
/// </summary>
public readonly record struct Error(string Code, string Message)
{
    /// <summary>
    /// Common singleton for unknown/unclassified errors.
    /// Prefer using domain-specific codes when possible.
    /// </summary>
    public static readonly Error None = new("None", "No error.");

    public bool IsNone => Code == "None";
}
