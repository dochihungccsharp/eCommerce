using eCommerce.Model.CartItems;

namespace eCommerce.Model.Shoppings;

public class EditShoppingModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Total { get; set; }
    public List<EditCartItemModel> EditCartItemModels { get; set; }
}