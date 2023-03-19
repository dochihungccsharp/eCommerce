namespace eCommerce.Domain.Abstractions.Audits;

public interface IFullAuditDomain: ICreatedAuditDomain, IModifiedAuditDomain, IIsDeletedAuditDomain, IStatusAuditDomain
{
    
}