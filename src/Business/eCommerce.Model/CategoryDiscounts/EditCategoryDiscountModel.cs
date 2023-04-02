using eCommerce.Model.CategoryProductExclusions;

namespace eCommerce.Model.CategoryDiscounts;

public class EditCategoryDiscountModel
{
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public IList<EditCategoryProductExclusionModel>? ProductExclusions { get; set; }
}