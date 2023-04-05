namespace eCommerce.Model.PurchaseOrderDetails;

public class PurchaseOrderDetailsModel
{
    public Guid PurchaseOrderId { get; set; }
    
    public Guid ProductId { get; set; }
        
    public string ProductName { get; set; }

    public int Quantity { get; set; }
    
    public decimal Price { get; set; }
}