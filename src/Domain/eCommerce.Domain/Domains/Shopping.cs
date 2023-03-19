using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class Shopping : ICreatedAuditDomain, IModifiedAuditDomain
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Total { get; set; }

    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    #endregion
}