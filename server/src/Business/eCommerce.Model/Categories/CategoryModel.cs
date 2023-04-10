namespace eCommerce.Model.Categories;

public class CategoryModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public bool Status { get; set; }
    public Guid ParentId { get; set; }
    public DateTime Created { get; set; }
}