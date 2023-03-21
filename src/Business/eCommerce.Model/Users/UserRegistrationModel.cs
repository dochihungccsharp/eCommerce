using Newtonsoft.Json;

namespace eCommerce.Model.Users;

public class UserRegistrationModel
{
    [JsonProperty("full_name")]
    public string FullName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("contact_number")]
    public string? ContactNumber { get; set; }
}