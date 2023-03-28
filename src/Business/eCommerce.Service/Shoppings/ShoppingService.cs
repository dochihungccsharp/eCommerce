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
using eCommerce.Service.Products;
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
    private readonly IProductService _productService;
    private readonly UserContextModel _userContextModel;
    private const string SQL_QUERY = "sp_Shoppings";
    
    public ShoppingService(
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
    
    // get shopping cart details
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
                {"Activity", "GET_SHOPPING_DETAILS_BY_USER_ID"},
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

        if (editShoppingModel.EditCartItemModels != null && editShoppingModel.EditCartItemModels.Count > 0)
        {
            var duplicateCartItem = editShoppingModel.EditCartItemModels.HasDuplicated(x => x.ProductId);
            if(duplicateCartItem) 
                throw new BadRequestException("Duplicate item found in the cart.");
        }

        var shopping = await FindShoppingByUserId(userId, cancellationToken).ConfigureAwait(false);
        if (shopping == null)
            throw new NotFoundException("The shopping is not found");

        await _databaseRepository.ExecuteAsync(
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
        
        return new BaseResponseModel("Update shopping cart success");
    }
    
    public async Task<BaseResponseModel> AddCartItemAsync(EditCartItemModel cartItemModel, CancellationToken cancellationToken = default)
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

        if (cartItemModel.Quantity > p.Quantity)
            throw new BadRequestException("Insufficient product inventory");
        
        var shopping = await FindShoppingByUserId(u.Id, cancellationToken).ConfigureAwait(false);
        if (shopping != null)
        {
            await _databaseRepository.ExecuteAsync(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    {"Activity", "ADD_CART_ITEM"},
                    {"Id", shopping.Id },
                    {"ProductId", cartItemModel.ProductId},
                    {"Quantity", cartItemModel.Quantity }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }
        else
        {
            await _databaseRepository.ExecuteAsync(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    {"Activity", "CREATE_SHOPPING"},
                    {"Id", Guid.NewGuid()},
                    {"UserId", u.Id},
                    {"Total", 0 },
                    {"CartItems", new List<EditCartItemModel>(){ cartItemModel }.ToDataTable()}
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }
        return new BaseResponseModel("Product added to cart successfully");

    }

    public async Task<BaseResponseModel> UpdateCartItemAsync(Guid cartItemId ,EditCartItemModel cartItemModel, CancellationToken cancellationToken = default)
    {
        if(_userContextModel == null)
            throw new BadRequestException("The request is invalid");
        
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");
        
        var cartItem = await FindCartItemByIdAsync(cartItemId, cancellationToken).ConfigureAwait(false);
        if (cartItemId == null)
            throw new BadRequestException("The cart item is not found");
        
        var p = await _productService.FindByIdAsync(cartItemModel.ProductId, cancellationToken).ConfigureAwait(false);
        if (p == null)
            throw new BadRequestException("The product is not found");

        if (cartItemModel.Quantity <= p.Quantity)
            throw new BadRequestException("Insufficient product inventory");

        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "UPDATE_CART_ITEM"},
                {"Id", cartItem.Id },
                {"ShoppingId", cartItem.ShoppingId },
                {"ProductId", cartItemModel.ProductId },
                {"Quantity", cartItemModel.Quantity }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    
        return new BaseResponseModel("Update cart item successfully");

    }



    public async Task<BaseResponseModel> DeleteCartItemAsync(Guid cartItemId , CancellationToken cancellationToken)
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
                {"Activity", "DELETE_CART_ITEM"},
                {"Id", cartItem.Id }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    
        return new BaseResponseModel("Update cart item successfully");
        
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
    

    public async Task<Shopping> FindShoppingByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var shopping = await _databaseRepository.GetAsync<Shopping>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_SHOPPING_BY_USER_ID"},
                {"UserId", userId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return shopping;
    }
}