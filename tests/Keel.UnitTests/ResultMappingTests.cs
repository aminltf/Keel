using Keel.Kernel.Core.Primitives;
using Keel.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Keel.UnitTests;

public class ResultMappingTests
{
    [Fact]
    public void ToActionResult_Success_NoContent_204()
    {
        var r = Result.Success();
        IActionResult ar = r.ToActionResult(null);
        var sc = Assert.IsType<StatusCodeResult>(ar);
        Assert.Equal(StatusCodes.Status204NoContent, sc.StatusCode);
    }

    [Fact]
    public void ToActionResult_Failure_Returns_ProblemDetails_404()
    {
        var r = Result.Failure("Employees.NotFound", "x");
        var ctx = new DefaultHttpContext();
        var ar = r.ToActionResult(ctx);
        var obj = Assert.IsType<ObjectResult>(ar);
        Assert.Equal(404, obj.StatusCode);
        var pd = Assert.IsType<ProblemDetails>(obj.Value);
        Assert.Equal("Not Found", pd.Title);
    }

    [Fact]
    public async Task ToIResult_Failure_Writes_ProblemDetails()
    {
        var r = Result.Failure("Validation.Failed", "bad");
        var ires = r.ToIResult();

        var ctx = new DefaultHttpContext();
        var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails() // IProblemDetailsService
            .BuildServiceProvider();
        ctx.RequestServices = services;

        ctx.Response.Body = new MemoryStream();

        await ires.ExecuteAsync(ctx);

        Assert.Equal(422, ctx.Response.StatusCode);
        Assert.Equal("application/problem+json", ctx.Response.ContentType);
        ctx.Response.Body.Position = 0;
        var json = new StreamReader(ctx.Response.Body).ReadToEnd();
        Assert.Contains("\"title\":\"Validation Failed\"", json);
    }
}
