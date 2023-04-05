using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class ReturnOrder : IAuditDomain
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalMoney { get; set; }
    public string ReturnOrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public decimal TotalPaymentAmount { get; set; }
    
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion
}