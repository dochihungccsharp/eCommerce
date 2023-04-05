using eCommerce.Model.Abstractions.Audits;

namespace eCommerce.Model.Brands;

public class BrandDetailsModel : IAuditModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LogoURL { get; set; }
    public string Description { get; set; }
    public bool Status { get; set; }

    #region [AUDIT PROPERTIES]
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    #endregion
}