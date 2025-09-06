using Keel.Kernel.Exceptions;
using Keel.Web.Errors;
using Keel.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Keel.UnitTests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task ExceptionMiddleware_Writes_ProblemDetails_And_Headers()
    {
        var logger = new NullLogger<ExceptionHandlingMiddleware>();
        var mapper = new ExceptionProblemDetailsMapper();
        var mw = new ExceptionHandlingMiddleware(logger, mapper);

        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/employees/123";
        ctx.Request.Headers["X-Correlation-Id"] = "corr-1";

        ctx.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new NotFoundException("Employee", "123");

        await mw.InvokeAsync(ctx, next);

        Assert.Equal(404, ctx.Response.StatusCode);
        Assert.True(ctx.Response.Headers.ContainsKey("X-Trace-Id"));
        Assert.Equal("corr-1", ctx.Response.Headers["X-Correlation-Id"].ToString());

        ctx.Response.Body.Position = 0;
        var json = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("/employees/123", doc.RootElement.GetProperty("instance").GetString());
        Assert.Equal("Not Found", doc.RootElement.GetProperty("title").GetString());
        Assert.Equal("corr-1", doc.RootElement.GetProperty("correlationId").GetString());
        Assert.True(doc.RootElement.TryGetProperty("traceId", out _));
    }
}
