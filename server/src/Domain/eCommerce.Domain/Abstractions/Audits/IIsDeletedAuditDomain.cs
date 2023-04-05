namespace eCommerce.Domain.Abstractions.Audits;

public interface IIsDeletedAuditDomain
{
    public bool IsDeleted { get; set; }
}