using eCommerce.Model.CartItems;
using eCommerce.Model.Users;

namespace eCommerce.Model.Carts;

public class CartDetailsModel
{
    public decimal Total { get; set; }
    public List<CartItemModel> _CartItems { get; set; }
}