using Microsoft.AspNetCore.Identity;

namespace ProductWebApp.Data;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}

