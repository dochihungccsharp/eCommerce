using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.UserAddresses;

namespace eCommerce.Service.UserAddresses;

public interface IUserAddressService
{
    Task<OkResponseModel<IEnumerable<UserAddressModel>>> GetAllByUserIdAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<UserAddressModel>> GetAsync(Guid userAddressId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateAsync(EditUserAddressModel editUserAddressModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateAsync(Guid userAddressId, EditUserAddressModel editUserAddressModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteAsync(Guid userAddressId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> SetDefaultAddressForUserAsync(Guid userAddressId, CancellationToken cancellationToken = default);
}