using eCommerce.Model.CartItems;
using eCommerce.Service.Shoppings;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class ShoppingController : BaseController
{
    private readonly IShoppingService _shoppingService;
    public ShoppingController(ILogger<ShoppingController> logger, IShoppingService shoppingService) : base(logger)
    {
        _shoppingService = shoppingService;
    }

    [HttpGet]
    [Route("api/shopping")]
    public async Task<IActionResult>GetShoppingDetailsAsync(
        CancellationToken cancellationToken = default)
        => Ok(await _shoppingService.GetShoppingDetailsAsync(cancellationToken).ConfigureAwait(false));

    [HttpPost]
    [Route("api/shopping/cart-item")]
    public async Task<IActionResult> AddCartItemAsync(EditCartItemModel editCartItemModel,
        CancellationToken cancellationToken = default)
        => Ok(await _shoppingService.AddCartItemAsync(editCartItemModel, cancellationToken).ConfigureAwait(false));
    
    [HttpPut]
    [Route("api/shopping/cart-item/{id:guid}")]
    public async Task<IActionResult> UpdateCartItemAsync([FromRoute(Name = "id")]Guid cartItemId ,EditCartItemModel editCartItemModel,
        CancellationToken cancellationToken = default)
        => Ok(await _shoppingService.UpdateCartItemAsync(cartItemId, editCartItemModel, cancellationToken).ConfigureAwait(false));
    
    [HttpDelete]
    [Route("api/shopping/cart-item/{id:guid}")]
    public async Task<IActionResult> DeleteCartItemAsync([FromRoute(Name = "id")]Guid cartItemId ,
        CancellationToken cancellationToken = default)
        => Ok(await _shoppingService.DeleteCartItemAsync(cartItemId, cancellationToken).ConfigureAwait(false));
}