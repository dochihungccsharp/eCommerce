using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.RoleRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Infrastructure.UserRoleRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CartItems;
using eCommerce.Model.Shoppings;
using eCommerce.Model.Users;
using eCommerce.Service.AccessToken;
using eCommerce.Service.SendMail;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Service.Shoppings;

public class ShoppingService : IShoppingService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserContextModel _userContextModel;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private const string SQL_QUERY = "sp_Shoppings";
    
    public ShoppingService(
        IDatabaseRepository databaseRepository,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        UserContextModel userContextModel,
        IMapper mapper,
        IWebHostEnvironment env
    )
    {
        _databaseRepository = databaseRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _userContextModel = userContextModel ?? throw new BadRequestException("The request is invalid");
        _mapper = mapper;
        _env = env;
    }
    public async Task<OkResponseModel<ShoppingDetailsModel>> GetShoppingDetailsAsync(CancellationToken cancellationToken = default)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        var shopping = await _databaseRepository.GetAsync<ShoppingDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_SHOPPING_BY_USER_ID"},
                {"UserId", u.Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new OkResponseModel<ShoppingDetailsModel>(shopping);

    }

    public async Task<BaseResponseModel> UpdateShoppingAsync(EditShoppingModel editShoppingModel, CancellationToken cancellationToken = default)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");

        HashSet<Guid> itemSet = new HashSet<Guid>();
        foreach (EditCartItemModel item in editShoppingModel.EditCartItemModels)
        {
            if (!itemSet.Add(item.ProductId))
            {
                throw new BadRequestException("Duplicate item found in the cart.");
            }
        }
        
        var shopping = await FindShoppingByUserId(userId, cancellationToken).ConfigureAwait(false);
        if (shopping == null)
            throw new NotFoundException("The shopping is not found");

        var resultUpdate = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "UPDATE_SHOPPING"},
                {"Id", editShoppingModel.Id},
                {"UserId", editShoppingModel.UserId},
                {"Total", editShoppingModel.Total},
                {"EditCartItemModels", editShoppingModel.EditCartItemModels?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultUpdate)
            throw new InternalServerException("Update shopping cart fail");

        return new BaseResponseModel("Update shopping cart success");
    }

    public async Task<BaseResponseModel> AddCartItemAsync(EditCartItemModel cartItemModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> UpdateCartItemAsync(EditCartItemModel cartItemModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> RemoveCartItemAsync(EditCartItemModel editCartItemModel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Shopping> FindShoppingByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var shopping = await _databaseRepository.GetAsync<Shopping>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_SHOPPING_BY_USER_ID"},
                {"UserId", userId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return shopping;
    }
}