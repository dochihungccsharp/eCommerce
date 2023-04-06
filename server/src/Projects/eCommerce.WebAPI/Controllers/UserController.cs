using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;
using eCommerce.Service.Users;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class UserController : BaseController
{
    private readonly IUserService _userService;
    public UserController(
        ILogger<UserController> logger,
        IUserService userService
        ) : base(logger)
    {
        _userService = userService;
    }

    #region Accounts API
    [HttpPost]
    [ProducesResponseType(typeof(AuthorizedResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] UserLoginModel loginModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.SignInAsync(loginModel, cancellationToken).ConfigureAwait(false));

    
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/users/sign-up")]
    public async Task<IActionResult> SignUpAsync([FromBody] UserRegistrationModel registerModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.SignUpAsync(registerModel, cancellationToken).ConfigureAwait(false));
    
    
    [Authorize]
    [HttpPut]
    [Route("api/users/refresh-token")]
    [ProducesResponseType(typeof(AuthorizedResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken = default)
        => Ok(await _userService.RefreshTokenAsync(cancellationToken).ConfigureAwait(false));
    
    [HttpGet]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/users/confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery(Name = "user_id")]Guid userId, [FromQuery(Name = "code")]string code, CancellationToken cancellationToken)
        => Ok(await _userService.ConfirmEmailAsync(userId, code, cancellationToken));

    
    [HttpPut]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/users/forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromQuery(Name = "email")]string email,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.ForgotPasswordAsync(email, cancellationToken).ConfigureAwait(false));
    #endregion

    #region Users API (Role Admin)
    [HttpGet]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/users")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetAllAsync([FromQuery] UserFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.GetAllAsync(filter, cancellationToken).ConfigureAwait(false));

    [Filters.Authorize(Roles.Admin)]
    [HttpGet]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/users/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid userId,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.GetAsync(userId, cancellationToken).ConfigureAwait(false));

    [Authorize(Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    [Route("api/users")]
    public async Task<IActionResult> CreateAsync([FromBody]EditUserModel editUserModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.CreateAsync(editUserModel, cancellationToken).ConfigureAwait(false));

    [Authorize(Roles.Admin)]
    [HttpPut]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")]Guid userId, [FromBody] EditUserModel editUserModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.UpdateAsync(userId, editUserModel, cancellationToken).ConfigureAwait(false));

    [Authorize(Roles.Admin)]
    [HttpDelete]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid userId,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.DeleteAsync(userId, cancellationToken).ConfigureAwait(false));
    #endregion

    #region Users API (Role Member)

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/profile")]
    public async Task<IActionResult> GetProfileAsync(CancellationToken cancellationToken = default)
        => Ok(await _userService.GetProfileAsync(cancellationToken).ConfigureAwait(false));

    [Authorize]
    [HttpPut]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody]ChangePasswordModel changePasswordModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.ChangePasswordAsync(changePasswordModel, cancellationToken).ConfigureAwait(false));
    
    [Authorize]
    [HttpPut]
    [ProducesResponseType(typeof(BaseResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/profile")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody]EditProfileModel editProfileModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.UpdateProfileAsync(editProfileModel, cancellationToken).ConfigureAwait(false));
    #endregion
}