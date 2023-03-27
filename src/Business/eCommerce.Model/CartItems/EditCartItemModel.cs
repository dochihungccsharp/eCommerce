namespace eCommerce.Model.CartItems;

public class EditCartItemModel
{
    public Guid ShoppingId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}