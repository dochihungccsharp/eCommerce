using eCommerce.Domain.Abstractions.Audits;
using eCommerce.Domain.Abstractions.Paginations;

namespace eCommerce.Domain.Domains;

public class CategoryDiscount : IAuditDomain, IPagedDomain
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion

    #region Paged domain
    public int TotalRows { get; set; }
    #endregion
}