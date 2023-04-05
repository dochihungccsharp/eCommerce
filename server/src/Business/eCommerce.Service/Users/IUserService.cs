using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Paginations;
using eCommerce.Model.UserAddresses;
using eCommerce.Model.Users;

namespace eCommerce.Service.Users;

public interface IUserService
{
    #region Accounts Service
    Task<BaseResponseModel> SignUpAsync(UserRegistrationModel registerUser, CancellationToken cancellationToken = default);
    Task<AuthorizedResponseModel> SignInAsync(UserLoginModel loginUser, CancellationToken cancellationToken = default);
    Task<AuthorizedResponseModel> RefreshTokenAsync(CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ConfirmEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    #endregion

    #region User Service (Admin)
    Task<OkResponseModel<PaginationModel<UserModel>>> GetAllAsync(UserFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<UserProfileModel>> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateAsync(EditUserModel editUserModel,CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateAsync(Guid userId, EditUserModel editUserModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    #endregion

    #region User Service (Member)
    Task<OkResponseModel<UserProfileModel>> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ChangePasswordAsync(ChangePasswordModel changePasswordModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateProfileAsync(EditProfileModel editProfileModel,CancellationToken cancellationToken = default);
    #endregion

}