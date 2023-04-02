using eCommerce.Model.Abstractions.Audits;
using eCommerce.Model.Categories;
using eCommerce.Model.Users;

namespace eCommerce.Model.CategoryDiscounts;

public class CategoryDiscountModel : IAuditModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    #region Audit model
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    #endregion
}