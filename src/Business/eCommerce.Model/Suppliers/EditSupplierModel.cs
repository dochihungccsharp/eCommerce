namespace eCommerce.Model.Suppliers;

public class EditSupplierModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? ContactPerson { get; set; }
    public bool? Status { get; set; }
}