namespace eCommerce.Model.Roles;

public class RoleModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}