namespace eCommerce.Domain.Domains;

public class Ward
{
    public Guid WardId { get; set; }
    public Guid DistrictId { get; set; }
    public string Name { get; set; }
    public bool Status { get; set; }
}