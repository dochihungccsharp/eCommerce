using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.UserAddresses;

namespace eCommerce.Service.UserAddresses;

public class UserAddress : IUserAddress
{
    public async Task<OkResponseModel<IList<UserAddressModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OkResponseModel<UserAddressModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> CreateUserAddressAsync(EditUserAddressModel editUserAddressModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> UpdateUserAddressAsync(Guid userAddressId, EditUserAddressModel editUserAddressModel,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> DeleteUserAddressAsync(Guid userAddressId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> SetDefaultUserAddressAsync(Guid userAddressId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}