using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CartItems;
using eCommerce.Model.Carts;

namespace eCommerce.Service.Carts;

public interface ICartService
{
    Task<OkResponseModel<CartDetailsModel>> GetCartDetailsAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<CartDetailsModel>> GetCartItemsAsync(List<Guid> cartItems,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> AddOrUpdateCartItemAsync(EditCartItemModel cartItemModel, 
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> RemoveCartItemAsync(Guid cartItemId,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> RemoveCartItemsAsync(List<Guid> cartItemIds,
        CancellationToken cancellationToken = default);
    Task<CartItem> FindCartItemByIdAsync(Guid Id, CancellationToken cancellationToken = default);

}