using eCommerce.Model.Users;

namespace eCommerce.Model.Promotions;

public class PromotionDetailsModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserModel _User { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}