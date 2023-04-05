using eCommerce.Model.Products;

namespace eCommerce.Model.OrderItems;

public class OrderItemModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public ProductModel _Product { get; set; }
}