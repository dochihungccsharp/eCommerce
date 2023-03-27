using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CartItems;
using eCommerce.Model.Shoppings;

namespace eCommerce.Service.Shoppings;

public interface IShoppingService
{
    Task<OkResponseModel<ShoppingDetailsModel>> GetShoppingDetailsAsync(CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateShoppingAsync(EditShoppingModel editShoppingModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> AddCartItemAsync(EditCartItemModel cartItemModel, 
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> UpdateCartItemAsync(EditCartItemModel cartItemModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> RemoveCartItemAsync(EditCartItemModel editCartItemModel,
        CancellationToken cancellationToken);

}