using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class Order : IAuditDomain
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid PromotionId { get; set; }
    public decimal Total { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }
    public string OrderStatus { get; set; }
    public string Note { get; set; }
    public bool IsCancelled { get; set; }
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion
}