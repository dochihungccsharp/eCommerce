using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CartItems;
using eCommerce.Model.Carts;
using eCommerce.Model.Users;
using eCommerce.Service.Products;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;

namespace eCommerce.Service.Carts;

public class CartService : ICartService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductService _productService;
    private readonly UserContextModel _userContextModel;
    private const string SQL_QUERY = "sp_Carts";
    
    public CartService(
        IDatabaseRepository databaseRepository,
        IUserRepository userRepository,
        IProductService productService,
        UserContextModel userContextModel
    )
    {
        _databaseRepository = databaseRepository;
        _userRepository = userRepository;
        _productService = productService;
        _userContextModel = userContextModel ?? throw new BadRequestException("The request is invalid");
    }
    
    public async Task<OkResponseModel<CartDetailsModel>> GetCartDetailsAsync(CancellationToken cancellationToken = default)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        var cart = await _databaseRepository.GetAsync<CartDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_CART_BY_USER_ID"},
                {"UserId", u.Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (cart == null)
            throw new NotFoundException("User's cart does not exist");
        
        return new OkResponseModel<CartDetailsModel>(cart);

    }

    public async Task<OkResponseModel<CartDetailsModel>> GetCartItemsAsync(List<Guid> cartItemIds, CancellationToken cancellationToken = default)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        if (cartItemIds == null || cartItemIds.Count < 1)
            throw new BadRequestException("The list of items in the cart is not found");
        
        var duplicateCartItem = cartItemIds.HasDuplicated(x => x);
        if(duplicateCartItem)
            throw new BadRequestException("The list of items in the cart is duplicated");
        
        var cart = await _databaseRepository.GetAsync<CartDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_CART_ITEMS_BY_USER_ID"},
                {"UserId", userId},
                {"CartItems", cartItemIds?.Select(x => new CartItemIdModel(){Id = x}).ToList().ToDataTable() }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    
        return new OkResponseModel<CartDetailsModel>(cart);
        
    }

    public async Task<BaseResponseModel> AddOrUpdateCartItemAsync(EditCartItemModel cartItemModel, CancellationToken cancellationToken = default)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");

        var p = await _productService.FindByIdAsync(cartItemModel.ProductId, cancellationToken).ConfigureAwait(false);
        if (p == null)
            throw new BadRequestException("The product is not found");

        if (cartItemModel.Quantity < 1)
            throw new BadRequestException("Quantity must be greater than 0");
        
        if (cartItemModel.Quantity > p.Quantity)
            throw new BadRequestException("Insufficient product inventory");

        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "ADD_OR_UPDATE_CART_ITEM"},
                {"UserId", u.Id},
                {"ProductId", cartItemModel.ProductId},
                {"Quantity", cartItemModel.Quantity }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Product added to cart successfully");

    }
    
    public async Task<BaseResponseModel> RemoveCartItemAsync(Guid cartItemId , CancellationToken cancellationToken)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        var cartItem = await FindCartItemByIdAsync(cartItemId, cancellationToken).ConfigureAwait(false);
        if (cartItem == null)
            throw new BadRequestException("The cart item is not found");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "REMOVE_CART_ITEM"},
                {"Id", cartItem.Id }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    
        return new BaseResponseModel("Update cart item successfully");
        
    }
    
    public async Task<BaseResponseModel> RemoveCartItemsAsync(List<Guid> cartItemIds, CancellationToken cancellationToken)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");

        if (cartItemIds == null || cartItemIds.Count < 1)
            throw new BadRequestException("The list of items in the cart is not found");
        
        var duplicateCartItem = cartItemIds.HasDuplicated(x => x);
        if(duplicateCartItem)
            throw new BadRequestException("The list of items in the cart is duplicated");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "REMOVE_CART_ITEMS"},
                {"CartItems", cartItemIds.Select(x => new CartItemIdModel(){Id = x}).ToList().ToDataTable() }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    
        return new BaseResponseModel("Delete the cart item list successfully");
        
    }

    public async Task<CartItem> FindCartItemByIdAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        var cartItem = await _databaseRepository.GetAsync<CartItem>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_CART_ITEM_BY_ID"},
                {"Id", Id}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return cartItem;
    }
}