using eCommerce.Model.CartItems;
using eCommerce.Service.Carts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class CartController : BaseController
{
    private readonly ICartService _cartService;
    public CartController(ILogger<CartController> logger, ICartService cartService) : base(logger)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [Route("api/carts")]
    [Authorize]
    public async Task<IActionResult>GetCartDetailsAsync(
        CancellationToken cancellationToken = default)
        => Ok(await _cartService.GetCartDetailsAsync(cancellationToken).ConfigureAwait(false));
    
    [HttpGet]
    [Route("api/carts/cart-items")]
    [Authorize]
    public async Task<IActionResult> GetCartItemsAsync([FromBody]List<Guid> cartItemIds ,
        CancellationToken cancellationToken = default)
        => Ok(await _cartService.GetCartItemsAsync(cartItemIds, cancellationToken).ConfigureAwait(false));

    [HttpPost]
    [Route("api/carts/cart-items")]
    [Authorize]
    public async Task<IActionResult> AddOrUpdateCartItemAsync(EditCartItemModel editCartItemModel,
        CancellationToken cancellationToken = default)
        => Ok(await _cartService.AddOrUpdateCartItemAsync(editCartItemModel, cancellationToken).ConfigureAwait(false));
    
    [HttpDelete]
    [Route("api/carts/cart-items/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveCartItemAsync([FromRoute(Name = "id")]Guid cartItemId ,
        CancellationToken cancellationToken = default)
        => Ok(await _cartService.RemoveCartItemAsync(cartItemId, cancellationToken).ConfigureAwait(false));
    
    [HttpDelete]
    [Route("api/carts/cart-items/delete-list")]
    [Authorize]
    public async Task<IActionResult> DeleteCartItemAsync([FromBody]List<Guid> cartItemIds ,
        CancellationToken cancellationToken = default)
        => Ok(await _cartService.RemoveCartItemsAsync(cartItemIds, cancellationToken).ConfigureAwait(false));
}