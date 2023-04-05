using Microsoft.AspNetCore.Http;

namespace eCommerce.Model.Users;

public class EditProfileModel
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string? Address { get; set; }
    public Guid? UserAddressId { get; set; }
}