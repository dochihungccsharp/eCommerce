using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Model.Abstractions.Audits;

public interface IFilterRequestAuditModel
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? SearchString { get; set; }
}