using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class OrderItem : ICreatedAuditDomain, IModifiedAuditDomain
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
 
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    #endregion
}