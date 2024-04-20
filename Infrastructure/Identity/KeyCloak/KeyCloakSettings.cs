namespace Infrastructure.Identity.KeyCloak;

public class KeyCloakSettings
{
    public string? BaseUrl { get; set; }
    public string? BaseUrlLogin { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}
