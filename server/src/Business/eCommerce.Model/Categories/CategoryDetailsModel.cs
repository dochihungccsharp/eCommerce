namespace eCommerce.Model.Categories;

public class CategoryDetailsModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public Guid? ParentId { get; set; }
    public CategoryModel? _Category{ get; set; }
    public bool Status { get; set; }

    #region [AUDIT PROPERTIES]
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    #endregion
}