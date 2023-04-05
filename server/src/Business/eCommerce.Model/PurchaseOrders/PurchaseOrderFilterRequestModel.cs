using eCommerce.Model.Abstractions.Audits;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Model.Models.PurchaseOrder;

[BindProperties]
public class PurchaseOrderFilterRequestModel : IFilterRequestAuditModel
{
    [BindProperty(Name = "page_index")] 
    public int PageIndex { get; set; } = 1;
    
    [BindProperty(Name = "page_size")]
    public int PageSize { get; set; } = 10;
    
    [BindProperty(Name = "search_string")]
    public string? SearchString { get; set; }
    
    [BindProperty(Name = "creator_id")]
    public Guid? UserId { get; set; }

    [BindProperty(Name = "supplier_id")]
    public string? SupplierId { get; set; }

    [BindProperty(Name = "purchase_order_note")]
    public string? PurchaseOrderNote { get; set; }
    
    [BindProperty(Name = "purchase_order_status")]
    public string? PurchaseOrderStatus { get; set; }
    
    [BindProperty(Name = "from_date")]
    public DateTime? FromTime { get; set; }
    
    [BindProperty(Name = "to_date")]
    public DateTime? ToTime { get; set; }
    
    [BindProperty(Name = "from_price")]
    public DateTime? FromPrice { get; set; }
    
    [BindProperty(Name = "to_price")]
    public DateTime? ToPrice { get; set; }
    
}
