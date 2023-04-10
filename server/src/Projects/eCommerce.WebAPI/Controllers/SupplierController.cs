using eCommerce.Model.Suppliers;
using eCommerce.Service.Suppliers;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class SupplierController : BaseController
{
    private readonly ISupplierService _supplierService;
    public SupplierController(ILogger<SupplierController> logger, ISupplierService supplierService) : base(logger)
    {
        _supplierService = supplierService;
    }

    #region API Public
    [HttpGet]
    [Route("api/suppliers")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] SupplierFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _supplierService.GetAllAsync(filter, cancellationToken)
            .ConfigureAwait(false));
    #endregion

    #region API Private
    [HttpGet]
    [Route("api/suppliers/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid supplierId,
        CancellationToken cancellationToken = default)
        => Ok(await _supplierService.GetAsync(supplierId, cancellationToken).ConfigureAwait(false));
    
    [HttpGet]
    [Route("api/suppliers/{id:guid}/details")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetDetailsAsync([FromRoute(Name = "id")] Guid supplierId,
        CancellationToken cancellationToken = default)
        => Ok(await _supplierService.GetDetailsAsync(supplierId, cancellationToken).ConfigureAwait(false));
    
    [HttpPost]
    [Route("api/suppliers")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditSupplierModel editSupplierModel,
        CancellationToken cancellationToken)
        => Ok(await _supplierService.CreateAsync(editSupplierModel, cancellationToken).ConfigureAwait(false));
    

    [HttpPut]
    [Route("api/suppliers/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid supplierId,
        [FromBody] EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default)
        => Ok(await _supplierService.UpdateAsync(supplierId, editSupplierModel, cancellationToken).ConfigureAwait(false));

    [HttpDelete]
    [Route("api/suppliers/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid supplierId, CancellationToken cancellationToken = default)
        => Ok(await _supplierService.DeleteAsync(supplierId, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/suppliers/{id:guid}/status")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> ChangeStatusAsync([FromRoute(Name = "id")] Guid supplierId, CancellationToken cancellationToken = default)
        => Ok(await _supplierService.ChangeStatusAsync(supplierId, cancellationToken).ConfigureAwait(false));
    
    #endregion
}