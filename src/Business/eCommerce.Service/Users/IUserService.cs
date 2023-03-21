using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;

namespace eCommerce.Service.Users;

public interface IUserService
{
    Task<BaseResponseModel> SignUpAsync(UserRegistrationModel registerUser, CancellationToken cancellationToken = default);
    Task<AuthorizedResponseModel> SignInAsync(UserLoginModel loginUser, CancellationToken cancellationToken = default);
    Task<OkResponseModel<UserProfileModel>> GetProfileAsync(CancellationToken cancellationToken = default);
}