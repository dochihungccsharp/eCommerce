using Microsoft.AspNetCore.Http;

namespace eCommerce.Model.Categories;

public class EditCategoryModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IFormFile? ImageUpload { get; set; }
    public Guid? ParentId { get; set; }
    public bool? Status { get; set; }
}