namespace eCommerce.Model.PurchaseOrderDetails;

public class EditPurchaseOrderDetailsModel
{
    public Guid ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }
}