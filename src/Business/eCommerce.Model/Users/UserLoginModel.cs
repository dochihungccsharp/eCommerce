using Newtonsoft.Json;

namespace eCommerce.Service.Users;

public class UserLoginModel
{
    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
}