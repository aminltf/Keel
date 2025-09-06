using Microsoft.AspNetCore.Http;

namespace Keel.Web.Errors;

/// <summary>
/// Default mapping between domain exception families and HTTP status codes & types (RFC 7807).
/// You can override these in DI if needed.
/// </summary>
public static class ProblemDetailsDefaults
{
    public const string TypePrefix = "https://problems.example.com/"; // change to your domain

    public static int MapStatusCode(Exception ex) =>
        ex switch
        {
            Kernel.Exceptions.UnauthorizedException => StatusCodes.Status401Unauthorized,
            Kernel.Exceptions.ForbiddenException => StatusCodes.Status403Forbidden,
            Kernel.Exceptions.NotFoundException => StatusCodes.Status404NotFound,
            Kernel.Exceptions.ValidationException => StatusCodes.Status422UnprocessableEntity,
            Kernel.Exceptions.ConcurrencyException => StatusCodes.Status409Conflict,
            Kernel.Exceptions.ConflictException => StatusCodes.Status409Conflict,
            Kernel.Exceptions.BadRequestException => StatusCodes.Status400BadRequest,
            Kernel.Exceptions.BusinessRuleException => StatusCodes.Status422UnprocessableEntity,
            Kernel.Exceptions.InternalServerException => StatusCodes.Status500InternalServerError,
            Kernel.Exceptions.DomainException => StatusCodes.Status400BadRequest, // generic domain error
            _ => StatusCodes.Status500InternalServerError
        };

    public static string MapType(Exception ex)
    {
        var code = ex switch
        {
            Keel.Kernel.Exceptions.DomainException de => de.Code,
            _ => "System.Internal"
        };
        return $"{TypePrefix}{code}";
    }

    public static string MapTitle(Exception ex) =>
        ex switch
        {
            Keel.Kernel.Exceptions.UnauthorizedException => "Unauthorized",
            Keel.Kernel.Exceptions.ForbiddenException => "Forbidden",
            Keel.Kernel.Exceptions.NotFoundException => "Not Found",
            Keel.Kernel.Exceptions.ValidationException => "Validation Failed",
            Keel.Kernel.Exceptions.ConcurrencyException => "Concurrency Conflict",
            Keel.Kernel.Exceptions.ConflictException => "Conflict",
            Keel.Kernel.Exceptions.BadRequestException => "Bad Request",
            Keel.Kernel.Exceptions.BusinessRuleException => "Business Rule Violation",
            Keel.Kernel.Exceptions.InternalServerException => "Internal Server Error",
            Keel.Kernel.Exceptions.DomainException => "Domain Error",
            _ => "Unhandled Error"
        };
}
