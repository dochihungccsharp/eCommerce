using eCommerce.Domain.Domains;
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

    Task<BaseResponseModel> UpdateCartItemAsync(Guid cartItemId ,EditCartItemModel cartItemModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> DeleteCartItemAsync(Guid cartItemId ,
        CancellationToken cancellationToken);
    Task<CartItem> FindCartItemByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<Shopping> FindShoppingByUserId(Guid userId, CancellationToken cancellationToken);

}