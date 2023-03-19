namespace eCommerce.Domain.Domains;

public class ReturnOrderDetail
{
    public Guid ReturnOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}