namespace eCommerce.Domain.Abstractions.Audits;

public interface ICreatedAuditDomain
{
    public DateTime Created { get; set; }
}