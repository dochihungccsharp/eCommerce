namespace eCommerce.Model.Shoppings;

public class ShoppingModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Total { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}