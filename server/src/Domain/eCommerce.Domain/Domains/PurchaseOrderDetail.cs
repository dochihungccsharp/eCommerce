namespace eCommerce.Domain.Domains;

public class PurchaseOrderDetail
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}