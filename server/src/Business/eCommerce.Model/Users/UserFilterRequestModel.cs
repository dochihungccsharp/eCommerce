using eCommerce.Model.Abstractions.Audits;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Model.Users;

[BindProperties]
public class UserFilterRequestModel : IFilterRequestAuditModel
{
    [BindProperty(Name = "page_index")] 
    public int PageIndex { get; set; } = 1;

    [BindProperty(Name = "page_size")] 
    public int PageSize { get; set; } = 10;
    [BindProperty(Name = "search_string")]
    public string? SearchString { get; set; }
}