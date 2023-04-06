using eCommerce.Model.Orders;
using eCommerce.Service.Orders;
using eCommerce.Service.Users;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class OrderController : BaseController
{
    private readonly IOrderService _orderService;
    public OrderController(ILogger<OrderController> logger,
        IOrderService orderService
    ) : base(logger)
    {
        _orderService = orderService;
    }

    #region Admin or Customer
    [HttpGet]
    [Route("api/orders/user/{id:guid}")]
    [Authorize]
    public async  Task<IActionResult> GetAllOrderByUserIdAsync([FromRoute(Name = "id")]Guid userId,
        CancellationToken cancellationToken = default)
        => Ok(await _orderService.GetAllOrderByUserId(userId, cancellationToken).ConfigureAwait(false));
    
    [HttpGet]
    [Route("api/orders/{id:guid}")]
    [Authorize]
    public async  Task<IActionResult> GetOrderDetailsAsync([FromRoute(Name = "id")]Guid orderId,
        CancellationToken cancellationToken = default)
        => Ok(await _orderService.GetOrderDetailsAsync(orderId, cancellationToken).ConfigureAwait(false));

    #endregion

    #region Admin
    [HttpGet]
    [Route("api/orders")]
    [Authorize(Roles.Admin)]
    public async  Task<IActionResult> GetAllOrder([FromQuery] OrderFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _orderService.GetAllOrder(filter, cancellationToken).ConfigureAwait(false));
    
    [HttpPut]
    [Route("api/orders/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async  Task<IActionResult> UpdateOrderAsync([FromRoute(Name = "id")]Guid orderId, [FromBody] UpdateOrderModel updateOrderModel,
        CancellationToken cancellationToken = default)
        => Ok(await _orderService.UpdateOrderAsync(orderId, updateOrderModel, cancellationToken).ConfigureAwait(false));

    #endregion

    #region Customer
    [HttpPost]
    [Route("api/orders")]
    [Authorize]
    public async Task<IActionResult> OrderAsync([FromBody] CreateOrderModel createOrderModel,
        CancellationToken cancellationToken = default)
        => Ok(await _orderService.OrderAsync(createOrderModel, cancellationToken).ConfigureAwait(false));
    
    [HttpPost]
    [Route("api/orders/{id:guid}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelOrderAsync([FromRoute(Name = "id")]Guid orderId,
        CancellationToken cancellationToken = default)
        => Ok(await _orderService.CancelOrderAsync(orderId, cancellationToken).ConfigureAwait(false));
    #endregion
}