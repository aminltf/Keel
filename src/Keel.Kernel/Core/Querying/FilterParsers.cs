using System.Globalization;

namespace Keel.Kernel.Core.Querying;

/// <summary>Common string-to-type parsers for filters (InvariantCulture by default).</summary>
public static class FilterParsers
{
    public static bool TryGuid(string s, out Guid v) =>
        Guid.TryParse(s, out v);

    public static bool TryInt32(string s, out int v) =>
        int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

    public static bool TryInt64(string s, out long v) =>
        long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

    public static bool TryDecimal(string s, out decimal v) =>
        decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v);

    public static bool TryDouble(string s, out double v) =>
        double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out v);

    public static bool TryDateTime(string s, out DateTime v) =>
        DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out v);

    public static bool TryDateTimeOffset(string s, out DateTimeOffset v) =>
        DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out v);
}
