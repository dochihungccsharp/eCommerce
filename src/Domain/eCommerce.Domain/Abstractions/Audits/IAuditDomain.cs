namespace eCommerce.Domain.Abstractions.Audits;

public interface IAuditDomain : ICreatedAuditDomain, IModifiedAuditDomain, IIsDeletedAuditDomain
{
    
}