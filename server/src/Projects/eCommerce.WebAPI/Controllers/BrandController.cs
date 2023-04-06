using eCommerce.Model.Brands;
using eCommerce.Service.Brands;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class BrandController : BaseController
{
    private readonly IBrandService _brandService;

    public BrandController(ILogger<BrandController> logger, IBrandService brandService) : base(logger)
    {
        _brandService = brandService;
    }

    #region API Public
    
    [HttpGet]
    [Route("api/brands")]
    public async Task<IActionResult> GetAllAsync([FromQuery]BrandFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _brandService.GetAllAsync(filter, cancellationToken).ConfigureAwait(false));
    
    [HttpGet]
    [Route("api/brands/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid brandId,
        CancellationToken cancellationToken = default)
        => Ok(await _brandService.GetAsync(brandId, cancellationToken).ConfigureAwait(false));

    [HttpGet]
    [Route("api/brands/{id:guid}/details")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute(Name = "id")] Guid brandId,
        CancellationToken cancellationToken = default)
        => Ok(await _brandService.GetDetailsAsync(brandId, cancellationToken).ConfigureAwait(false));
    
    #endregion

    #region API Private

    [HttpPost]
    [Route("api/brands")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditBrandModel editBrandModel,
        CancellationToken cancellationToken)
    {
        return Ok(await _brandService.CreateAsync(editBrandModel, cancellationToken).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/brands/{id:guid}")]
        [Authorize(Roles.Admin)]

    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid brandId,
        [FromBody] EditBrandModel editBrandModel, CancellationToken cancellationToken = default)
    {
        return Ok(await _brandService.UpdateAsync(brandId, editBrandModel, cancellationToken).ConfigureAwait(false));
    }

    [HttpDelete]
    [Authorize(Roles.Admin)]
    [Route("api/brands/{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid brandId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _brandService.DeleteAsync(brandId, cancellationToken).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/brands/{id:guid}/brand-status")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> ChangeStatusAsync([FromRoute(Name = "id")] Guid brandId,
        CancellationToken cancellationToken)
    {
        return Ok(await _brandService.ChangeStatusAsync(brandId, cancellationToken).ConfigureAwait(false));
    }
    #endregion
    

}