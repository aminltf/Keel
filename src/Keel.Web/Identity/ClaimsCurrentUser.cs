using System.Security.Claims;
using Keel.Kernel.Abstractions.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Keel.Web.Execution;

namespace Keel.Web.Identity;

/// <summary>
/// Claims-based implementation of ICurrentUser{TUserKey}.
/// </summary>
public sealed class ClaimsCurrentUser<TUserKey, TTenantId> : ICurrentUser<TUserKey>
    where TUserKey : IEquatable<TUserKey>
    where TTenantId : IEquatable<TTenantId>
{
    private readonly IHttpContextAccessor _http;
    private readonly OperationContextOptions<TUserKey, TTenantId> _opts;

    public ClaimsCurrentUser(IHttpContextAccessor http, IOptions<OperationContextOptions<TUserKey, TTenantId>> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    private ClaimsPrincipal? Principal => _http.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public TUserKey? UserId
    {
        get
        {
            var raw = Principal?.FindFirstValue(_opts.UserIdClaimType);
            if (string.IsNullOrWhiteSpace(raw)) return default;
            return _opts.ParseUserId(raw);
        }
    }

    public string? UserName => Principal?.FindFirstValue(_opts.UserNameClaimType);
    public string? Email => Principal?.FindFirstValue(_opts.EmailClaimType);

    public IEnumerable<string> Roles =>
        Principal?.Claims.Where(c => c.Type == _opts.RoleClaimType).Select(c => c.Value)
        ?? Enumerable.Empty<string>();
}
