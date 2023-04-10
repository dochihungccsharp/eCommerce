namespace eCommerce.Model.Products;

public class ProductModel
{
    public Guid Id { get; set; }
        
    public string Name { get; set; }
        
    public string Slug { get; set; }
        
    public string Description { get; set; }
        
    public string ImageUrl { get; set; }
  
    public decimal OriginalPrice { get; set; }

    public decimal Price { get; set; }
        
    public int Quantity { get; set; }
        
    public int QuantitySold { get; set; }
    
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? SupplierId { get; set; }
        
    public bool Status { get; set; }
        
    public bool IsBestSelling { get; set; }
        
    public bool IsNew { get; set; }
}