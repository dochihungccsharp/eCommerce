namespace eCommerce.Domain.Domains;

public class District
{
    public Guid DistrictId { get; set; }
    public Guid ProvinceId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
}