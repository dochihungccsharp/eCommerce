using eCommerce.Model.OrderItems;
using eCommerce.Model.Promotions;
using eCommerce.Model.Users;

namespace eCommerce.Model.Orders;

public class OrderDetailsModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid PromotionId { get; set; }
    public decimal Total { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }
    public string OrderStatus { get; set; }
    public string Note { get; set; }
    public bool IsCancelled { get; set; }
    
    public UserModel _User { get; set; }
    public PromotionModel _Promotion { get; set; }
    public IList<OrderItemModel> _OrderItems { get; set; }


    #region Audit Model
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    #endregion

}