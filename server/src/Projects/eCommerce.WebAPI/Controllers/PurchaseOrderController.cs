using eCommerce.Model.Models.PurchaseOrder;
using eCommerce.Model.PurchaseOrders;
using eCommerce.Service.PurchaseOrders;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class PurchaseOrderController : BaseController
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrderController(ILogger<PurchaseOrderController> logger, IPurchaseOrderService purchaseOrderService)
        : base(logger)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    [HttpGet]
    [Route("api/purchase-orders")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] PurchaseOrderFilterRequestModel filter,
        CancellationToken cancellationToken)
        => Ok(await _purchaseOrderService.GetAllAsync(filter, cancellationToken)
            .ConfigureAwait(false));

    [HttpGet]
    [Route("api/purchase-orders/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid purchaseOrderId,
        CancellationToken cancellationToken = default)
        => Ok(await _purchaseOrderService.GetAsync(purchaseOrderId, cancellationToken)
            .ConfigureAwait(false));

    [HttpGet]
    [Route("api/purchase-orders/{id:guid}/details")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetDetailsAsync([FromRoute(Name = "id")] Guid purchaseOrderId,
        CancellationToken cancellationToken = default)
        => Ok(await _purchaseOrderService.GetDetailsAsync(purchaseOrderId, cancellationToken)
            .ConfigureAwait(false));

    [HttpPost]
    [Route("api/purchase-orders")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditPurchaseOrderModel editPurchaseOrderModel,
        CancellationToken cancellationToken = default)
        => Ok(await _purchaseOrderService.CreateAsync(editPurchaseOrderModel, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/purchase-orders/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid purchaseOrderId,
        [FromBody] EditPurchaseOrderModel editPurchaseOrderModel,
        CancellationToken cancellationToken = default)
        => Ok(await _purchaseOrderService.UpdateAsync(purchaseOrderId, editPurchaseOrderModel, cancellationToken)
            .ConfigureAwait(false));



    [HttpDelete]
    [Route("api/purchase-orders/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid purchaseOrderId,
        CancellationToken cancellationToken = default)
        => Ok(await _purchaseOrderService.DeleteAsync(purchaseOrderId, cancellationToken).ConfigureAwait(false));
}