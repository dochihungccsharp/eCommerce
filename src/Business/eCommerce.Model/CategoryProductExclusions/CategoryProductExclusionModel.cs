using eCommerce.Model.Products;

namespace eCommerce.Model.CategoryProductExclusions;

public class CategoryProductExclusionModel
{
    public Guid Id { get; set; }
    public Guid CategoryDiscountId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ProductId { get; set; }
    public ProductModel _Product { get; set; }
}