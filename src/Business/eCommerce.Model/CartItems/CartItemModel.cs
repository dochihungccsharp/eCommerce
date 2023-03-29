using eCommerce.Model.Products;

namespace eCommerce.Model.CartItems;

public class CartItemModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public ProductModel _Product { get; set; }
}