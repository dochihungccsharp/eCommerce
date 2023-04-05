namespace eCommerce.Domain.Domains;

public class CategoryProductExclusion
{
    public Guid Id { get; set; }
    public Guid CategoryDiscountId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ProductId { get; set; }
}