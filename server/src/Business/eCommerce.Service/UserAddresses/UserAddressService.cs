using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.UserAddresses;
using eCommerce.Model.Users;
using eCommerce.Shared.Exceptions;

namespace eCommerce.Service.UserAddresses;

public class UserAddressService : IUserAddressService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly UserContextModel _userContextModel;
    private const string SQL_QUERY = "sp_UserAddresses";

    public UserAddressService(
        IDatabaseRepository databaseRepository,
        IUserRepository userRepository,
        UserContextModel userContextModel
        )
    {
        _databaseRepository = databaseRepository;
        _userRepository = userRepository;
        _userContextModel = userContextModel ?? throw new BadRequestException("The request is invalid");
    }
    public async Task<OkResponseModel<IEnumerable<UserAddressModel>>> GetAllByUserIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        var addresses = await _databaseRepository.GetAllAsync<UserAddressModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_USER_ADDRESSES_BY_USER_ID"},
                {"UserId", u.Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        
        return new OkResponseModel<IEnumerable<UserAddressModel>>(addresses);
    }

    public async Task<OkResponseModel<UserAddressModel>> GetAsync(Guid userAddressId, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        var address = await _databaseRepository.GetAsync<UserAddressModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_USER_ADDRESS_BY_USER_ID"},
                {"Id", userAddressId},
                {"UserId", u.Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (address == null)
            throw new NotFoundException("The user address is not found");

        return new OkResponseModel<UserAddressModel>(address);
    }

    public async Task<BaseResponseModel> CreateAsync(EditUserAddressModel editUserAddressModel, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");

        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "CREATE_USER_ADDRESS"},
                {"Id", Guid.NewGuid() },
                {"UserId", u.Id},
                {"Name", editUserAddressModel.Name},
                {"DeliveryAddress", editUserAddressModel.DeliveryAddress },
                {"Telephone", editUserAddressModel.Telephone },
                {"Active", editUserAddressModel.Active }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Create user address success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid userAddressId, EditUserAddressModel editUserAddressModel,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "UPDATE_USER_ADDRESS"},
                {"Id", userAddressId},
                {"UserId", u.Id},
                {"Name", editUserAddressModel.Name},
                {"DeliveryAddress", editUserAddressModel.DeliveryAddress },
                {"Telephone", editUserAddressModel.Telephone },
                {"Active", editUserAddressModel.Active }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Update user address success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid userAddressId, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "DELETE_USER_ADDRESS"},
                {"Id", userAddressId },
                {"UserId", u.Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Delete user address success");
    }

    public async Task<BaseResponseModel> SetDefaultAddressForUserAsync(Guid userAddressId, CancellationToken cancellationToken = default)
    {

        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "SET_DEFAULT_ADDRESS_FOR_USER"},
                {"Id", userAddressId },
                {"UserId", u.Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Delete user address success");
    }
}