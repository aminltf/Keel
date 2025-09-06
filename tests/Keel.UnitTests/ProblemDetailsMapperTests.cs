using Keel.Kernel.Exceptions;
using Keel.Web.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Keel.UnitTests;

public class ProblemDetailsMapperTests
{
    [Fact]
    public void ExceptionMapper_Maps_NotFound_To_404()
    {
        var mapper = new ExceptionProblemDetailsMapper();
        var ex = new NotFoundException("Employee", "123");
        var pd = mapper.Map(ex, "/employees/123");

        Assert.Equal(404, pd.Status);
        Assert.Contains("NotFound", pd.Type);
        Assert.Equal("Not Found", pd.Title);
        Assert.Equal("/employees/123", pd.Instance);
        Assert.Equal("Employee.NotFound", pd.Extensions["code"]);
    }

    [Fact]
    public void ResultMapper_Maps_Error_Code_To_Proper_Status()
    {
        var mapper = new ResultProblemDetailsMapper();
        var error = new Keel.Kernel.Core.Primitives.Error("Auth.Unauthorized", "token missing");
        ProblemDetails pd = mapper.Map(error, "/me");

        Assert.Equal(401, pd.Status);
        Assert.Contains("Auth.Unauthorized", pd.Type);
        Assert.Equal("Unauthorized", pd.Title);
    }
}
