using Newtonsoft.Json;

namespace Application.Identity;

public class KeyCloakUser
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonProperty("createdTimestamp")]
    public long CreatedTimestamp { get; set; }

    [JsonProperty("enabled")]
    public bool Enabled { get; set; }
}