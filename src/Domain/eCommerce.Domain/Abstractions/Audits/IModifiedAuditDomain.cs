namespace eCommerce.Domain.Abstractions.Audits;

public interface IModifiedAuditDomain
{
    public DateTime Modified { get; set; }
}