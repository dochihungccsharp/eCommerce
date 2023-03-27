using eCommerce.Model.CartItems;
using eCommerce.Model.Users;

namespace eCommerce.Model.Shoppings;

public class ShoppingDetailsModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Total { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public UserModel _User { get; set; }
    public List<CartItemModel> _CartItems { get; set; }
}