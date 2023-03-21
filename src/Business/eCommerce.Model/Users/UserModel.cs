namespace eCommerce.Model.Users;

public class UserModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public bool EmailConfirm { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string TotalAmountOwed { get; set; }
    public string Address { get; set; }
    public Guid UserAddressId { get; set; }
    public bool Status { get; set; }
}