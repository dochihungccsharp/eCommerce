using Newtonsoft.Json;

namespace eCommerce.Model.Users;

public class UserRegistrationModel
{
    public string Fullname { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string? PhoneNumber { get; set; }
}