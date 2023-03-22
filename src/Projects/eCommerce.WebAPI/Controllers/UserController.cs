using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;
using eCommerce.Service.Users;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(AuthorizedResponseModel), StatusCodes.Status200OK)]
    [Route("api/users/sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] UserLoginModel loginModel,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.SignInAsync(loginModel, cancellationToken).ConfigureAwait(false));

    
    [AllowAnonymous]
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
    
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/accounts/confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery(Name = "user_id")]Guid userId, [FromQuery(Name = "code")]string code, CancellationToken cancellationToken)
        => Ok(await _userService.ConfirmEmailAsync(userId, code, cancellationToken));


    [AllowAnonymous]
    [HttpPut]
    [ProducesResponseType(typeof(OkResponseModel<BaseResponseModel>), StatusCodes.Status200OK)]
    [Route("api/accounts/forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromQuery(Name = "email")]string email,
        CancellationToken cancellationToken = default)
        => Ok(await _userService.ForgotPasswordAsync(email, cancellationToken).ConfigureAwait(false));
    #endregion

    #region Users API (Role Admin)

    

    #endregion

    #region Users API (Role Member)

    

    #endregion
}