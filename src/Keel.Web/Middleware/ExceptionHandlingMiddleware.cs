using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Keel.Web.Errors;
using Keel.Kernel.Exceptions;

namespace Keel.Web.Middleware;

/// <summary>
/// Global exception handling middleware producing RFC 7807 ProblemDetails responses,
/// enriched with trace and correlation identifiers for observability.
/// Place early in the pipeline.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ExceptionProblemDetailsMapper _mapper;

    // Response/Request header names
    private const string CorrelationHeader = "X-Correlation-Id";
    private const string TraceHeader       = "X-Trace-Id";

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        ExceptionProblemDetailsMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext http, Exception ex)
    {
        var instancePath = http.Request.Path.HasValue ? http.Request.Path.Value : null;

        // Log with appropriate severity
        if (ex is DomainException)
            _logger.LogWarning(ex, "Domain exception at {Path}", instancePath);
        else
            _logger.LogError(ex, "Unhandled exception at {Path}", instancePath);

        // Map to ProblemDetails
        var pd = _mapper.Map(ex, instancePath);

        // --- Enrich with tracing/correlation identifiers ---
        // W3C TraceId (preferred) or server-generated request id
        var traceId = Activity.Current?.TraceId.ToString() ?? http.TraceIdentifier;

        // CorrelationId from incoming header (if any) or generate a new one
        var incomingCorr = http.Request.Headers[CorrelationHeader].ToString();
        var correlationId = string.IsNullOrWhiteSpace(incomingCorr)
            ? Guid.NewGuid().ToString("N")
            : incomingCorr;

        pd.Extensions["traceId"] = traceId;
        pd.Extensions["correlationId"] = correlationId;

        // Echo identifiers in response headers for client-side support workflows
        http.Response.Headers[TraceHeader] = traceId;
        http.Response.Headers[CorrelationHeader] = correlationId;

        // Write ProblemDetails
        http.Response.ContentType = "application/problem+json";
        http.Response.StatusCode  = pd.Status ?? StatusCodes.Status500InternalServerError;

        var payload = _mapper.Serialize(pd);
        await http.Response.WriteAsync(payload);
    }
}
