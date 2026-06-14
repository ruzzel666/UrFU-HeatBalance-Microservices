using Microsoft.AspNetCore.Identity;

namespace HeatBalance.Auth;

public sealed class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
