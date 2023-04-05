using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class UserAddress : ICreatedAuditDomain, IModifiedAuditDomain, IIsDeletedAuditDomain
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string DeliveryAddress { get; set; }
    public string Telephone { get; set; }
    public bool Active { get; set; }

    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion
}