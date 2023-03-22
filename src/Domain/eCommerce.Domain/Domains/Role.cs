using eCommerce.Domain.Abstractions.Audits;

namespace eCommerce.Domain.Domains;

public class Role : IAuditDomain
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    #region Audit Domain
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion
}