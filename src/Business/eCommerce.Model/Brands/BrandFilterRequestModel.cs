using eCommerce.Model.Abstractions.Audits;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Model.Brands;

[BindProperties]
public class BrandFilterRequestModel : IFilterRequestAuditModel
{
    [BindProperty(Name = "page_index")] 
    public int PageIndex { get; set; }
    [BindProperty(Name = "page_size")] 
    public int PageSize { get; set; }
    [BindProperty(Name = "search_string")]
    public string? SearchString { get; set; }
}