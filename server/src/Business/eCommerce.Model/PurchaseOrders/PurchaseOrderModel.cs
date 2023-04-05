namespace eCommerce.Model.PurchaseOrders;

public class PurchaseOrderModel
{
    public Guid Id { get; set; }

    public string SupplierName { get; set; }
    
    public decimal TotalMoney { get; set; }
    
    public string Note { get; set; }
    
    public string OrderStatus { get; set; }
    
    public string UserName { get; set; }
    
    public DateTime Created { get; set; }

}