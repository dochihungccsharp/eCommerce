namespace eCommerce.Model.Abstractions.Audits;

public interface IAuditModel
{
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}