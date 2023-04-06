using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Roles;
using eCommerce.Service.Roles;
using eCommerce.Shared.Consts;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class RoleController  : BaseController
{
    private readonly IRoleService _roleService;
    
    public RoleController(ILogger<RoleController> logger, IRoleService roleService) : base(logger)
    {
        _roleService = roleService;
    }

    #region Roles API (Role Admin) Private
    [HttpGet]
    [Route("api/roles")]
    [Filters.Authorize(Roles.Admin)]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        => Ok(await _roleService.GetAllAsync(cancellationToken).ConfigureAwait(false));

    [HttpGet]
    [Route("api/roles/{id:guid}")]
    [Filters.Authorize(Roles.Admin)]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery(Name = "id")]Guid roleId ,CancellationToken cancellationToken = default)
        => Ok(await _roleService.GetAsync(roleId, cancellationToken).ConfigureAwait(false));

    [HttpPost]
    [Route("api/roles")]
    [Filters.Authorize(Roles.Admin)]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] EditRoleModel editRoleModel,
        CancellationToken cancellationToken = default)
        => Ok(await _roleService.CreateAsync(editRoleModel, cancellationToken).ConfigureAwait(false));
    
    [HttpPut]
    [Route("api/roles/{id:guid}")]
    [Filters.Authorize(Roles.Admin)]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")]Guid roleId,[FromBody] EditRoleModel editRoleModel,
        CancellationToken cancellationToken = default)
        => Ok(await _roleService.UpdateAsync(roleId, editRoleModel, cancellationToken).ConfigureAwait(false));
    
    [HttpDelete]
    [Route("api/roles/{id:guid}")]
    [Filters.Authorize(Roles.Admin)]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")]Guid roleId,
        CancellationToken cancellationToken = default)
        => Ok(await _roleService.DeleteAsync(roleId, cancellationToken).ConfigureAwait(false));
    #endregion
}