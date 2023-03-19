using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class OrderDetail : IAuditDomain
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Total { get; set; }
    
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion
}