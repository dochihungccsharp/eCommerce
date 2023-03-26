using eCommerce.Model.PurchaseOrderDetails;

namespace eCommerce.Model.PurchaseOrders;

public class EditPurchaseOrderModel
{
    public Guid SupplierId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalMoney { get; set; }
    public string Note { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public IList<EditPurchaseOrderDetailsModel> EditPurchaseOrderDetailsModels { get; set; }
}