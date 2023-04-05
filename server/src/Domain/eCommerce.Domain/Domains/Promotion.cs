using eCommerce.Domain.Abstractions.Audits;
using eCommerce.Domain.Abstractions.Paginations;

namespace eCommerce.Domain.Domains;

public class Promotion : IPagedDomain
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalRows { get; set; }
}