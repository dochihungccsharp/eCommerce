using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.UserAddresses;

namespace eCommerce.Service.UserAddresses;

public interface IUserAddress
{
    Task<OkResponseModel<IList<UserAddressModel>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<UserAddressModel>> GetAsync(CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateUserAddressAsync(EditUserAddressModel editUserAddressModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateUserAddressAsync(Guid userAddressId, EditUserAddressModel editUserAddressModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteUserAddressAsync(Guid userAddressId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> SetDefaultUserAddressAsync(Guid userAddressId, CancellationToken cancellationToken = default);
}