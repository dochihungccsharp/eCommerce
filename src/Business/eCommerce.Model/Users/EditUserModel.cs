using eCommerce.Model.Roles;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace eCommerce.Model.Users;

public class EditUserModel
{
    public string Username { get; set; }
    public string? Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string? Address { get; set; }
    public Guid? UserAddressId { get; set; }
    public List<AddRoleModel>? Roles { get; set; }
}

