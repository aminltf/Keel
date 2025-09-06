using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Keel.Kernel.Exceptions;

namespace Keel.Web.Errors;

/// <summary>
/// Translates domain exceptions into RFC 7807 ProblemDetails.
/// </summary>
public sealed class ExceptionProblemDetailsMapper
{
    private readonly JsonSerializerOptions _json;

    public ExceptionProblemDetailsMapper(JsonSerializerOptions? json = null)
    {
        _json = json ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public ProblemDetails Map(Exception ex, string? instancePath)
    {
        var status = ProblemDetailsDefaults.MapStatusCode(ex);
        var type = ProblemDetailsDefaults.MapType(ex);
        var title = ProblemDetailsDefaults.MapTitle(ex);

        var pd = new ProblemDetails
        {
            Status = status,
            Type = type,
            Title = title,
            Detail = ex.Message,
            Instance = instancePath
        };

        // Enrich with domain code & details if available
        if (ex is DomainException de)
        {
            pd.Extensions["code"] = de.Code;
            if (de.Details.Count > 0)
                pd.Extensions["details"] = de.Details; // serializable key-value
        }

        return pd;
    }

    public string Serialize(ProblemDetails details) =>
        JsonSerializer.Serialize(details, _json);
}
