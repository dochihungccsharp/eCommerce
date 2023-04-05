using eCommerce.Model.UserAddresses;
using eCommerce.Service.UserAddresses;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class UserAddressController : BaseController
{
    private readonly IUserAddressService _userAddressService;
    public UserAddressController(ILogger<UserAddressController> logger, IUserAddressService userAddressService) : base(logger)
    {
        _userAddressService = userAddressService;
    }
    
    [HttpGet]
    [Route("api/user-addresses")]
    [Authorize]
    public async Task<IActionResult> GetAllByUserIdAsync(CancellationToken cancellationToken = default)
        => Ok(await _userAddressService.GetAllByUserIdAsync(cancellationToken));
    
    [HttpGet]
    [Route("api/user-addresses/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")]Guid userAddressId, CancellationToken cancellationToken = default)
        => Ok(await _userAddressService.GetAsync(userAddressId, cancellationToken));
    
    [HttpPost]
    [Route("api/user-addresses")]
    [Authorize]
    public async Task<IActionResult> CreateAsync(EditUserAddressModel editUserAddressModel, CancellationToken cancellationToken = default)
        => Ok(await _userAddressService.CreateAsync(editUserAddressModel, cancellationToken));

    [HttpPut]
    [Route("api/user-addresses/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid userAddressId,
        EditUserAddressModel editUserAddressModel, CancellationToken cancellationToken = default)
        => Ok(await _userAddressService.UpdateAsync(userAddressId, editUserAddressModel, cancellationToken));

    [HttpDelete]
    [Route("api/user-addresses/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid userAddressId,
        CancellationToken cancellationToken = default)
        => Ok(await _userAddressService.DeleteAsync(userAddressId, cancellationToken));

    [HttpDelete]
    [Route("api/user-addresses/set-default/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> SetDefaultAddressForUserAsync([FromRoute(Name = "id")] Guid userAddressId,
        CancellationToken cancellationToken = default)
        => Ok(await _userAddressService.SetDefaultAddressForUserAsync(userAddressId, cancellationToken));


}