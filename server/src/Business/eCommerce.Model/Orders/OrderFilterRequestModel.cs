using eCommerce.Model.Abstractions.Audits;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Model.Orders;

[BindProperties]
public class OrderFilterRequestModel : IFilterRequestAuditModel
{
    [BindProperty(Name = "page_index")] 
    public int PageIndex { get; set; } = 1;
        
    [BindProperty(Name = "page_size")]
    public int PageSize { get; set; } = 10;

    [BindProperty(Name = "search_string")]
    public string? SearchString { get; set; }

    [BindProperty(Name = "from_date")]
    public DateTime? FromTime { get; set; }
        
    [BindProperty(Name = "to_date")]
    public DateTime? ToTime { get; set; }
        
    [BindProperty(Name = "from_price")]
    public decimal? FromPrice { get; set; }
        
    [BindProperty(Name = "to_price")]
    public decimal? ToPrice { get; set; }
    
    [BindProperty(Name = "is_cancelled")]
    public bool? IsCancelled { get; set; }
    
}