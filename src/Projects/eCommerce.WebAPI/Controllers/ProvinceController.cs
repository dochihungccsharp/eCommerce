using eCommerce.Model.Abstractions.Responses;
using eCommerce.Service.Provinces;
using eCommerce.Service.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class ProvinceController : BaseController
{
    private readonly IProvinceService _provinceService;
    public ProvinceController(ILogger logger, IProvinceService provinceService) : base(logger)
    {
        _provinceService = provinceService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/provinces/provinces")]
    public async Task<IActionResult> GetAllProvinceAsync(CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllProvinceAsync(cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/provinces/districts")]
    public async Task<IActionResult> GetAllDistrictAsync(CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllDistrictAsync(cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/provinces/districts")]
    public async Task<IActionResult> GetAllDistrictByProvinceIdAsync([FromQuery(Name = "province_id")] Guid provinceId,
        CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllDistrictByProvinceIdAsync(provinceId, cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/provinces/wards")]
    public async Task<IActionResult> GetAllWardAsync(CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllWardAsync(cancellationToken).ConfigureAwait(false));
    
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/provinces/wards")]
    public async Task<IActionResult> GetAllWardByDistrictIdAsync([FromQuery(Name = "ward_id")]Guid wardId,
        CancellationToken cancellationToken = default)
        => Ok(await _provinceService.GetAllWardByDistrictIdAsync(wardId, cancellationToken).ConfigureAwait(false));
}