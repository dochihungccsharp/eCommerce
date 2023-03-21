using eCommerce.Domain.Abstractions.Audits;
using eCommerce.Domain.Abstractions.Paginations;

namespace eCommerce.Domain.Domains;

public class User : IFullAuditDomain, IPagedDomain
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string Address { get; set; }
    public decimal TotalAmountOwed { get; set; }
    public Guid UserAddressId { get; set; }
    
    #region Full Audit Domain
    public bool Status { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    #endregion

    #region Paged Domain
    public int TotalRows { get; set; }
    #endregion
}