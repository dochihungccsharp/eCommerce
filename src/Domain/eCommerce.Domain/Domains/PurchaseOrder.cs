using eCommerce.Domain.Abstractions.Audits;
using eCommerce.Domain.Abstractions.Paginations;

namespace eCommerce.Domain.Domains;

public class PurchaseOrder : IAuditDomain, IPagedDomain
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public decimal TotalMoney { get; set; }
    public string Note { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public decimal TotalPaymentAmount { get; set; }

    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion

    #region Paged Domain
    public int TotalRows { get; set; }
    #endregion
}