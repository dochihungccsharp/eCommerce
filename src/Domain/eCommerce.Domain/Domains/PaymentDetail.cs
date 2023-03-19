using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class PaymentDetail : IAuditDomain
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
    public string Provider { get; set; }
    public string Status { get; set; }
    
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion
}