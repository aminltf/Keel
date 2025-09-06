using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Keel.Web.Execution;
using Keel.Web.Execution.Correlation;

namespace Keel.Web.Middleware;

/// <summary>
/// Ensures a correlation id exists for the request (from header or generated),
/// stores it in the accessor, Activity, and echoes it to the response header.
/// </summary>
public sealed class CorrelationMiddleware<TUserKey, TTenantId> : IMiddleware
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    private readonly OperationContextOptions<TUserKey, TTenantId> _opts;
    private readonly CorrelationIdAccessor _corr;

    public CorrelationMiddleware(
        IOptions<OperationContextOptions<TUserKey, TTenantId>> opts,
        CorrelationIdAccessor corr)
    {
        _opts = opts.Value;
        _corr = corr;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var incoming = context.Request.Headers[_opts.CorrelationHeader].ToString();
        var id = string.IsNullOrWhiteSpace(incoming) ? _opts.GenerateCorrelationId() : incoming;

        _corr.Set(id);
        Activity.Current?.AddTag("correlation_id", id);

        context.Response.Headers[_opts.CorrelationHeader] = id;

        try { await next(context); }
        finally { _corr.Clear(); }
    }
}
