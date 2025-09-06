using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Keel.Kernel.Abstractions.Execution;
using Keel.Kernel.Abstractions.Identity;
using Keel.Kernel.Abstractions.Correlation;
using Keel.Web.Execution;
using Keel.Web.Execution.Correlation;
using Keel.Web.Identity;
using Keel.Web.Middleware;
using Keel.Web.Tenancy;

namespace Keel.Web.Extensions;

/// <summary>
/// DI and pipeline helpers to enable OperationContext in web apps.
/// </summary>
public static class OperationContextExtensions
{
    public static IServiceCollection AddKeelOperationContext<TUserKey, TTenantId>(
        this IServiceCollection services,
        Action<OperationContextOptions<TUserKey, TTenantId>> configure)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
    {
        services.AddHttpContextAccessor();

        // Options
        services.AddOptions<OperationContextOptions<TUserKey, TTenantId>>()
                .Configure(configure);

        // Accessors & Implementations
        services.AddSingleton<CorrelationIdAccessor>();
        services.AddSingleton<ICorrelationIdAccessor>(sp => sp.GetRequiredService<CorrelationIdAccessor>());
        services.AddScoped<ICurrentUser<TUserKey>, ClaimsCurrentUser<TUserKey, TTenantId>>();
        services.AddSingleton<IOperationContextAccessor<TUserKey, TTenantId>, OperationContextAccessor<TUserKey, TTenantId>>();
        services.AddScoped<ITenantResolver<TTenantId>, HeaderTenantResolver<TUserKey, TTenantId>>();

        // Middlewares
        services.AddTransient<CorrelationMiddleware<TUserKey, TTenantId>>();
        services.AddTransient<OperationContextMiddleware<TUserKey, TTenantId>>();

        return services;
    }

    /// <summary>Registers correlation and operation context middlewares in the right order.</summary>
    public static IApplicationBuilder UseKeelOperationContext<TUserKey, TTenantId>(this IApplicationBuilder app)
        where TUserKey : IEquatable<TUserKey>
        where TTenantId : IEquatable<TTenantId>
    {
        return app
            .UseMiddleware<CorrelationMiddleware<TUserKey, TTenantId>>()      // must be first
            .UseMiddleware<OperationContextMiddleware<TUserKey, TTenantId>>(); // then snapshot context
    }
}
