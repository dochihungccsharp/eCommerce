using AutoMapper;
using eCommerce.Infrastructure.RoleRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Infrastructure.UserRoleRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;

namespace eCommerce.Service.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;
    
    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
    }
    public Task<OkResponseModel<UserProfileModel>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<BaseResponseModel> SignUpAsync(UserRegistrationModel registerUser, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuthorizedResponseModel> SignInAsync(UserLoginModel loginUser, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
}