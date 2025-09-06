using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Keel.Web.Errors;
using Keel.Web.Middleware;

namespace Keel.Web.Extensions;

/// <summary>
/// Service and pipeline extensions to enable Keel-style exception handling.
/// </summary>
public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddKeelProblemDetails(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionProblemDetailsMapper>();
        services.AddTransient<ExceptionHandlingMiddleware>();
        return services;
    }

    /// <summary>Registers the global exception handling middleware.</summary>
    public static IApplicationBuilder UseKeelExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
