using eCommerce.Model.Roles;
using eCommerce.Model.UserAddresses;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Model.Users;

public class UserProfileModel
{
    public Guid Id { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string Address { get; set; }
    public string TotalAmountOwed { get; set; }
    public Guid UserAddressId { get; set; }
    public List<UserAddressModel> _UserAddresses { get; set; }
    public List<RoleModel> _Roles { get; set; }
    public bool Status { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
}