namespace eCommerce.Model.Brands;

public class BrandModel
{
    public Guid Id { get; set; }
        
    public string Name { get; set; }
    
    public string LogoURL { get; set; }
    
    public string Description { get; set; }
    
    public bool Status { get; set; }
    
    public DateTime Created { get; set; }
}