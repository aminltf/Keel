using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Keel.Kernel.Core.Primitives;

namespace Keel.Web.Errors;

/// <summary>
/// Translates <see cref="Result"/> / <see cref="Error"/> into RFC 7807 <see cref="ProblemDetails"/>.
/// Uses simple, convention-based classification by error code.
/// </summary>
public sealed class ResultProblemDetailsMapper
{
    private const StringComparison OrdIgnore = StringComparison.OrdinalIgnoreCase;

    public ProblemDetails Map(Error error, string? instancePath = null)
    {
        var code = string.IsNullOrWhiteSpace(error.Code) ? "System.Internal" : error.Code;
        var status = MapStatusCodeByCode(code);

        return new ProblemDetails
        {
            Status = status,
            Type = ProblemDetailsDefaults.TypePrefix + code, // e.g. https://problems.example.com/Entities.NotFound
            Title = TitleFromCode(code),
            Detail = error.Message,
            Instance = instancePath,
            Extensions =
            {
                ["code"] = code
            }
        };
    }

    /// <summary>Classifies an error code into an HTTP status code.</summary>
    public int MapStatusCodeByCode(string code)
    {
        if (code.EndsWith(".NotFound", OrdIgnore)) return StatusCodes.Status404NotFound;
        if (code.EndsWith(".Concurrency", OrdIgnore)) return StatusCodes.Status409Conflict;
        if (code.EndsWith(".Conflict", OrdIgnore)) return StatusCodes.Status409Conflict;
        if (code.EndsWith(".BadRequest", OrdIgnore) ||
            code.StartsWith("Request.Bad", OrdIgnore)) return StatusCodes.Status400BadRequest;
        if (code.StartsWith("Auth.Unauthorized", OrdIgnore)) return StatusCodes.Status401Unauthorized;
        if (code.StartsWith("Auth.Forbidden", OrdIgnore)) return StatusCodes.Status403Forbidden;
        if (code.StartsWith("Validation.", OrdIgnore) ||
            code.EndsWith(".Validation", OrdIgnore) ||
            code.Contains(".Invalid", OrdIgnore)) return StatusCodes.Status422UnprocessableEntity;

        // Default for expected domain failures
        return StatusCodes.Status400BadRequest;
    }

    private static string TitleFromCode(string code)
    {
        if (code.EndsWith(".NotFound", OrdIgnore)) return "Not Found";
        if (code.EndsWith(".Concurrency", OrdIgnore)) return "Concurrency Conflict";
        if (code.EndsWith(".Conflict", OrdIgnore)) return "Conflict";
        if (code.EndsWith(".BadRequest", OrdIgnore) || code.StartsWith("Request.Bad", OrdIgnore)) return "Bad Request";
        if (code.StartsWith("Auth.Unauthorized", OrdIgnore)) return "Unauthorized";
        if (code.StartsWith("Auth.Forbidden", OrdIgnore)) return "Forbidden";
        if (code.StartsWith("Validation.", OrdIgnore) ||
            code.EndsWith(".Validation", OrdIgnore) ||
            code.Contains(".Invalid", OrdIgnore)) return "Validation Failed";
        return "Domain Error";
    }
}
