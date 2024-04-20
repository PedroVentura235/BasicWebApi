using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsActive { get; set; }

    public string? ObjectId { get; set; }

    public string? KeyCloakId { get; set; }
}
