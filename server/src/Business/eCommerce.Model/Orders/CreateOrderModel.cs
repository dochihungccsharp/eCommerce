using eCommerce.Model.OrderItems;

namespace eCommerce.Model.Orders;

public class CreateOrderModel
{
    public Guid UserId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? PromotionId { get; set; }
    public decimal Total { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }
    public string Note { get; set; }
    public IList<EditOrderItemModel>? OrderItems { get; set; }
}