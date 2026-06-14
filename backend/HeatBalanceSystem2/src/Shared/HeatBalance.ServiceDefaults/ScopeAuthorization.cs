using System.Security.Claims;
using HeatBalance.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace HeatBalance.ServiceDefaults;

public static class ScopeAuthorization
{
    public static bool HasScope(this ClaimsPrincipal user, string scope)
    {
        if (user.IsInRole(AuthConstants.AdminRole))
            return true;

        var scopeClaim = user.FindFirst("scope")?.Value;
        if (string.IsNullOrWhiteSpace(scopeClaim))
            return false;

        return scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Any(s => string.Equals(s, scope, StringComparison.OrdinalIgnoreCase));
    }

    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string scope)
    {
        return builder.RequireAssertion(ctx => ctx.User.HasScope(scope));
    }
}
