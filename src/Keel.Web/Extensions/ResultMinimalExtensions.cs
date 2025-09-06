using Microsoft.AspNetCore.Http;
using Keel.Kernel.Core.Primitives;
using Keel.Web.Errors;

namespace Keel.Web.Extensions;

/// <summary>
/// Helpers to convert <see cref="Result"/> / <see cref="Result{T}"/> to Minimal API <see cref="IResult"/>.
/// On failure, returns RFC 7807 ProblemDetails with an appropriate status code.
/// </summary>
public static class ResultMinimalExtensions
{
    /// <summary>
    /// Maps a command-style <see cref="Result"/> to <see cref="IResult"/>.
    /// Success → empty response (default 204). Failure → ProblemDetails.
    /// </summary>
    public static IResult ToIResult(
        this Result result,
        HttpContext? http = null,
        int successStatusCode = StatusCodes.Status204NoContent)
    {
        if (result.IsSuccess)
        {
            return successStatusCode switch
            {
                StatusCodes.Status204NoContent => Results.NoContent(),
                StatusCodes.Status200OK => Results.Ok(),
                _ => Results.StatusCode(successStatusCode)
            };
        }

        var mapper = new ResultProblemDetailsMapper();
        var pd = mapper.Map(result.Error, http?.Request.Path.Value);
        return Results.Problem(pd.Detail, pd.Instance, pd.Status, pd.Title, pd.Type, pd.Extensions);
    }

    /// <summary>
    /// Maps a query-style <see cref="Result{T}"/> to <see cref="IResult"/>.
    /// Success → body with value (default 200). Failure → ProblemDetails.
    /// </summary>
    public static IResult ToIResult<T>(
        this Result<T> result,
        HttpContext? http = null,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return successStatusCode switch
            {
                StatusCodes.Status200OK => Results.Ok(result.Value),
                StatusCodes.Status201Created => Results.Created(http?.Request.Path.Value ?? string.Empty, result.Value),
                _ => Results.Json(result.Value!, statusCode: successStatusCode)
            };
        }

        var mapper = new ResultProblemDetailsMapper();
        var pd = mapper.Map(result.Error, http?.Request.Path.Value);
        return Results.Problem(pd.Detail, pd.Instance, pd.Status, pd.Title, pd.Type, pd.Extensions);
    }
}
