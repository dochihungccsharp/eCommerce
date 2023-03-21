using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;

namespace eCommerce.Service.AccessToken;

public interface IAccessTokenService
{
    AuthorizedResponseModel GenerateJwtToken(UserContextModel auth);
    UserContextModel ParseJwtToken(string token);
}