using eCommerce.Model.Products;
using eCommerce.Service.Products;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class ProductController : BaseController
{
    private readonly IProductService _productService;
    public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
    {
        _productService = productService;
    }

    #region API Public
    [HttpGet]
    [Route("api/products")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery]ProductFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _productService.GetAllAsync(filter, cancellationToken)
            .ConfigureAwait(false));

    [HttpGet]
    [Route("api/products/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid productId, 
        CancellationToken cancellationToken = default)
        => Ok(await _productService.GetAsync(productId, cancellationToken).ConfigureAwait(false));

    [HttpGet]
    [Route("api/products/{id:guid}/details")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute(Name = "id")] Guid productId,
        CancellationToken cancellationToken = default)
        => Ok(await _productService.GetDetailsAsync(productId, cancellationToken).ConfigureAwait(false));
    #endregion

    #region API Private

    [HttpPost]
    [Route("api/products")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditProductModel editProductModel,
        CancellationToken cancellationToken = default)
        => Ok(await _productService.CreateAsync(editProductModel, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/products/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid productId,
        [FromBody] EditProductModel editProductModel, CancellationToken cancellationToken = default)
        => Ok(await _productService.UpdateAsync(productId, editProductModel, cancellationToken).ConfigureAwait(false));


    [HttpPut]
    [Route("api/products/{id:guid}/best-selling-status")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> ChangeStatusIsBestSellingAsync([FromRoute(Name = "id")] Guid productId,
        CancellationToken cancellationToken = default)
        => Ok(await _productService.ChangeIsBestSellingAsync(productId, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/products/{id:guid}/newest-status")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> ChangeStatusIsNew([FromRoute(Name = "id")] Guid productId, 
        CancellationToken cancellationToken = default)
        => Ok( await _productService.ChangeIsNewAsync(productId, cancellationToken).ConfigureAwait(false));


    [HttpPut]
    [Route("api/products/{id:guid}/status")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> ChangeStatusAsync([FromRoute(Name = "id")] Guid productId,
        CancellationToken cancellationToken = default)
        => Ok(await _productService.ChangeStatusAsync(productId, cancellationToken).ConfigureAwait(false));
    
    
    [HttpDelete]
    [Route("api/products/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid productId, 
        CancellationToken cancellationToken = default)
        => Ok(await _productService.DeleteAsync(productId, cancellationToken).ConfigureAwait(false));


    [HttpDelete]
    [Route("api/products/delete-multiple")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteListAsync([FromBody] List<string> listProductId,
        CancellationToken cancellationToken = default)
        => Ok(await _productService.DeleteListAsync(listProductId, cancellationToken).ConfigureAwait(false));
    #endregion

}