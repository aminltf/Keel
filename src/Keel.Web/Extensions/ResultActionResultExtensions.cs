using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Keel.Kernel.Core.Primitives;
using Keel.Web.Errors;

namespace Keel.Web.Extensions;

/// <summary>
/// Helpers to convert <see cref="Result"/> / <see cref="Result{T}"/> into MVC
/// <see cref="ActionResult"/> responses. On failure, responses are rendered as
/// RFC 7807 <see cref="ProblemDetails"/> with an appropriate HTTP status code.
/// </summary>
public static class ResultActionResultExtensions
{
    /// <summary>
    /// Converts a command-style <see cref="Result"/> to an <see cref="IActionResult"/>.
    /// On success, returns an empty response with the provided success status code
    /// (defaults to <see cref="StatusCodes.Status204NoContent"/>). On failure, returns
    /// a <see cref="ProblemDetails"/> payload with an appropriate error status.
    /// </summary>
    /// <param name="result">The operation result to map.</param>
    /// <param name="http">
    /// Optional <see cref="HttpContext"/> used to populate the <c>instance</c> field in
    /// <see cref="ProblemDetails"/> (typically the request path).
    /// </param>
    /// <param name="successStatusCode">
    /// HTTP status code to use when <paramref name="result"/> is successful.
    /// Defaults to 204 (No Content).
    /// </param>
    /// <returns>
    /// <see cref="StatusCodeResult"/> on success; <see cref="ObjectResult"/> containing
    /// <see cref="ProblemDetails"/> on failure.
    /// </returns>
    public static IActionResult ToActionResult(
        this Result result,
        HttpContext? http = null,
        int successStatusCode = StatusCodes.Status204NoContent)
    {
        if (result.IsSuccess)
            return new StatusCodeResult(successStatusCode);

        var mapper = new ResultProblemDetailsMapper();
        var pd = mapper.Map(result.Error, http?.Request.Path.Value);
        return new ObjectResult(pd) { StatusCode = pd.Status };
    }

    /// <summary>
    /// Converts a query-style <see cref="Result{T}"/> to <see cref="ActionResult{T}"/>.
    /// On success, returns the value with the provided success status code
    /// (defaults to <see cref="StatusCodes.Status200OK"/>). On failure, returns
    /// a <see cref="ProblemDetails"/> payload with an appropriate error status.
    /// </summary>
    /// <typeparam name="T">The value type contained in the successful result.</typeparam>
    /// <param name="result">The operation result to map.</param>
    /// <param name="http">
    /// Optional <see cref="HttpContext"/> used to populate the <c>instance</c> field in
    /// <see cref="ProblemDetails"/>.
    /// </param>
    /// <param name="successStatusCode">
    /// HTTP status code to use when <paramref name="result"/> is successful.
    /// Defaults to 200 (OK).
    /// </param>
    /// <returns>
    /// <see cref="OkObjectResult"/> (or a custom <see cref="ObjectResult"/> with the given
    /// status) on success; <see cref="ObjectResult"/> containing <see cref="ProblemDetails"/>
    /// on failure.
    /// </returns>
    public static ActionResult<T> ToActionResult<T>(
        this Result<T> result,
        HttpContext? http = null,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            if (successStatusCode == StatusCodes.Status200OK)
                return new OkObjectResult(result.Value);
            return new ObjectResult(result.Value) { StatusCode = successStatusCode };
        }

        var mapper = new ResultProblemDetailsMapper();
        var pd = mapper.Map(result.Error, http?.Request.Path.Value);
        return new ObjectResult(pd) { StatusCode = pd.Status };
    }

    /// <summary>
    /// Converts a successful <see cref="Result{T}"/> into a 201 Created response
    /// with a <c>Location</c> header and body containing the created resource.
    /// On failure, returns a <see cref="ProblemDetails"/> payload with an appropriate status.
    /// </summary>
    /// <typeparam name="T">The value type contained in the successful result.</typeparam>
    /// <param name="result">The operation result to map.</param>
    /// <param name="location">
    /// The absolute or relative URI identifying the newly created resource to be set in the
    /// <c>Location</c> header.
    /// </param>
    /// <param name="http">
    /// Optional <see cref="HttpContext"/> used to populate the <c>instance</c> field in
    /// <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    /// <see cref="CreatedResult"/> on success; <see cref="ObjectResult"/> containing
    /// <see cref="ProblemDetails"/> on failure.
    /// </returns>
    public static ActionResult<T> ToCreatedActionResult<T>(
        this Result<T> result,
        string location,
        HttpContext? http = null)
    {
        if (result.IsSuccess)
            return new CreatedResult(location, result.Value);

        var mapper = new ResultProblemDetailsMapper();
        var pd = mapper.Map(result.Error, http?.Request.Path.Value);
        return new ObjectResult(pd) { StatusCode = pd.Status };
    }
}
