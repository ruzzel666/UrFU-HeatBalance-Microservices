using System.Security.Claims;
using HeatBalance.Contracts;
using ProductWebApp.Data;

namespace ProductWebApp.Utils;

public static class UserClaimsHelper
{
    public static string? GetUserId(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");

    public static bool IsAdmin(ClaimsPrincipal user)
        => user.IsInRole(DbSeed.AdminRoleName) || user.IsInRole(AuthConstants.AdminRole);
}
