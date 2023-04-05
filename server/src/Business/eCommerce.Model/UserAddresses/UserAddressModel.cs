namespace eCommerce.Model.UserAddresses;

public class UserAddressModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string DeliveryAddress { get; set; }
    public string Telephone { get; set; }
    public bool Active { get; set; }
}