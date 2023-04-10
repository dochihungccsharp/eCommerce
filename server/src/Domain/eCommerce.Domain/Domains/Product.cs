using eCommerce.Domain.Abstractions.Audits;
using eCommerce.Domain.Abstractions.Paginations;

namespace eCommerce.Domain.Domains;

public class Product : IFullAuditDomain, IPagedDomain
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
    public bool IsBestSelling { get; set; }
    public bool IsNew { get; set; }

    public Guid CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? InventoryId { get; set; }
    
    #region Full Audit Domain
    public bool Status { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion

    #region Paged Domain
    public int TotalRows { get; set; }
    #endregion
}