using eCommerce.Model.Abstractions.Audits;
using eCommerce.Model.Brands;
using eCommerce.Model.Categories;
using eCommerce.Model.Inventories;
using eCommerce.Model.Suppliers;

namespace eCommerce.Model.Products;

public class ProductDetailsModel : IAuditModel
{
    public Guid Id { get; set; }
        
    public string Name { get; set; }
        
    public string Slug { get; set; }
        
    public string Description { get; set; }
        
    public string ImageUrl { get; set; }

    public decimal OriginalPrice { get; set; }
        
    public decimal Price { get; set; }
        
    public int QuantitySold { get; set; }
        
    public bool Status { get; set; }
        
    public bool IsBestSelling { get; set; }
        
    public bool IsNew { get; set; }
        
    public Guid CategoryId { get; set; }
        
    public CategoryModel _Category { get; set; }

    public Guid? SupplierId { get; set; }
        
    public SupplierModel? _Supplier { get; set; }

    public Guid? BrandId { get; set; }
        
    public BrandModel? _Brand { get; set; }

    public Guid? InventoryId { get; set; }
    public InventoryModel? _Inventory { get; set; }
    
    #region [AUDIT PROPERTIES]
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    #endregion
}