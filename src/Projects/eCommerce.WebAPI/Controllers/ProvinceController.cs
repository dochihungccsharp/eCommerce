using eCommerce.Model.Abstractions.Responses;
using eCommerce.Service.Provinces;
using eCommerce.Service.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class ProvinceController : BaseController
{
    private readonly IProvinceService _provinceService;
    public ProvinceController(ILogger<ProvinceController> logger, IProvinceService provinceService) : base(logger)
    {
        _provinceService = provinceService;
    }

    #region API Public
    [AllowAnonymous]
    [HttpPost]
    [Route("api/provinces")]
    public async Task<IActionResult> GetAllProvinceAsync(CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllProvinceAsync(cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [Route("api/provinces/{id:guid}/districts")]
    public async Task<IActionResult> GetAllDistrictByProvinceIdAsync([FromRoute(Name = "id")] Guid provinceId,
        CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllDistrictByProvinceIdAsync(provinceId, cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [Route("api/districts")]
    public async Task<IActionResult> GetAllDistrictAsync(CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllDistrictAsync(cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [Route("api/districts/{id:guid}/wards")]
    public async Task<IActionResult> GetAllWardByDistrictIdAsync([FromRoute(Name = "id")]Guid districtId,
        CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllWardByDistrictIdAsync(districtId, cancellationToken).ConfigureAwait(false));

    [AllowAnonymous]
    [HttpPost]
    [Route("api/provinces/wards")]
    public async Task<IActionResult> GetAllWardAsync(CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllWardAsync(cancellationToken).ConfigureAwait(false));
    
    #endregion
}