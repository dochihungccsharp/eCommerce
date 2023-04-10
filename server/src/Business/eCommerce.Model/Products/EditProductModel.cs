using Microsoft.AspNetCore.Http;

namespace eCommerce.Model.Products;

public class EditProductModel
{
    public string Name { get; set; }
        
    public string? Slug { get; set; }
        
    public string? Description { get; set; }
        
    public string? ImageUrl { get; set; }
    
    public decimal OriginalPrice { get; set; }
        
    public decimal Price { get; set; }

        
    public Guid CategoryId { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? BrandId { get; set; }
    public bool? Status { get; set; }
    public bool? IsBestSelling { get; set; }
    public bool? IsNew { get; set; }
    
}