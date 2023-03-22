using System.Text.Encodings.Web;
using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.RoleRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Infrastructure.UserRoleRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Paginations;
using eCommerce.Model.UserAddresses;
using eCommerce.Model.Users;
using eCommerce.Service.AccessToken;
using eCommerce.Service.SendMail;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using InvalidOperationException = System.InvalidOperationException;

namespace eCommerce.Service.Users;

public class UserService : IUserService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISendMailService _sendMailService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly UserContextModel _userContextModel;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    
    public UserService(
        IDatabaseRepository databaseRepository,
        IUserRepository userRepository, 
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IHttpContextAccessor httpContextAccessor,
        ISendMailService sendMailService,
        IAccessTokenService accessTokenService,
        UserContextModel userContextModel,
        IMapper mapper,
        IWebHostEnvironment env
        )
    {
        _databaseRepository = databaseRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _httpContextAccessor = httpContextAccessor;
        _sendMailService = sendMailService;
        _accessTokenService = accessTokenService;
        _userContextModel = userContextModel;
        _mapper = mapper;
        _env = env;
    }

    #region Accounts Service
    public async Task<BaseResponseModel> SignUpAsync(UserRegistrationModel registerUser, CancellationToken cancellationToken = default)
    {

        var user = _mapper.Map<User>(registerUser);
        var userId = Guid.NewGuid();
        user.Id = userId;

        var duplicateUser = await _userRepository.CheckDuplicateAsync(user, cancellationToken).ConfigureAwait(false);
        if (duplicateUser)
            throw new InvalidOperationException("User with the same name already exists.");
        
        var resultCreated = await _userRepository.CreateUserAsync(
                user:user,
                null,
                cancellationToken:cancellationToken
            ).ConfigureAwait(false);
        if (!resultCreated)
            throw new InternalServerException("Created user fail");
        
        var emailBody = "Please confirm your email address <a href=\"#URL#\">Click here </a>";

        var scheme = _httpContextAccessor?.HttpContext?.Request.Scheme;
        var host = _httpContextAccessor?.HttpContext?.Request.Host;

        var code = registerUser.Email.Base64Encode();
        var callbackUrl = scheme + "://" + host + $"/api/confirm-email?user_id={userId}&code={code}";;

        var encodedCallbackUrl = HtmlEncoder.Default.Encode(callbackUrl);
        var body = emailBody.Replace("#URL#", encodedCallbackUrl);
            
        await _sendMailService.SendEmailAsync(registerUser.Email, "Confirm Email",body);

        return new BaseResponseModel("Register user success");
    }

    public async Task<AuthorizedResponseModel> SignInAsync(UserLoginModel loginUser, CancellationToken cancellationToken = default)
    {
        var email = loginUser.Email.Trim().ToLower();
        var password = loginUser.Password.Trim().HashMD5();
        
        var u = await _userRepository.FindUserByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        if (u == null)
            return new AuthorizedResponseModel("The email or password is invalid.");
            
        if(u.EmailConfirmed == false)
            return new AuthorizedResponseModel("Your account has not been verified");

        if(u.PasswordHash != password)
            return new AuthorizedResponseModel("The email or password is invalid.");

        var userContextModel = GetUserContextModel(u);
            
        return _accessTokenService.GenerateJwtToken(userContextModel);
    }

    public async Task<AuthorizedResponseModel> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var userName = _userContextModel.Username;
        var u = await _userRepository.FindUserByNameAsync(userName, cancellationToken).ConfigureAwait(false);
        if(u == null)
            return new AuthorizedResponseModel("The email or password is invalid.");

        var userContextModel = GetUserContextModel(u);
        return _accessTokenService.GenerateJwtToken(userContextModel);
    }

    public async Task<BaseResponseModel> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        email = email.ToLower().Trim();
        var u = await _userRepository.FindUserByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        if (u == null)
            return new AuthorizedResponseModel("The email is not found");

        var newPassword = new Random().Next(100000, 999999).ToString();
        u.PasswordHash = newPassword.HashMD5();
        var resultForgotPass = await _userRepository.UpdateUserAsync(
                user: u, 
                roles: null,
                cancellationToken
            ).ConfigureAwait(false);
        if (!resultForgotPass)
            throw new InternalServerException("Forgot password fail");
        
        var body = $"Your new password is {newPassword}, please login and change the password to ....";

        await _sendMailService.SendEmailAsync(email, "Forgot Password",body);

        return new BaseResponseModel("forgot password success");

    }

    public async Task<BaseResponseModel> ConfirmEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var email = code.Base64Decode().Trim().ToLower();
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null || String.Compare(u.Email, email, StringComparison.OrdinalIgnoreCase) != 0)
            throw new BadRequestException("confirm email fail");
        
        if (u.EmailConfirmed == true)
            return new BaseResponseModel("Account Verified");
        
        u.EmailConfirmed = true;

        var resultConfirm = await _userRepository.UpdateUserAsync(
                user: u, 
                roles: null,
                cancellationToken
            ).ConfigureAwait(false);
        if (!resultConfirm)
            throw new InternalServerException("Confirm email fail");
        return new BaseResponseModel("Confirm mail success");
    }
    #endregion
    
    #region User Service (Admin)
    public async Task<OkResponseModel<PaginationModel<UserModel>>> GetAllAsync(UserFilterRequestModel filter, CancellationToken cancellationToken = default)
    {
        var users = await _databaseRepository.PagingAllAsync<User>(
            sqlQuery: "sp_GetAllUser",
            pageIndex: filter.PageIndex,
            pageSize: filter.PageSize,
            parameters: new Dictionary<string, object>()
            {
                { "SearchString", filter.SearchString }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<PaginationModel<UserModel>>(_mapper.Map<PaginationModel<UserModel>>(users));
    }

    public async Task<OkResponseModel<UserProfileModel>> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _databaseRepository.GetAsync<UserProfileModel>(
            sqlQuery: "sp_GetUserProfile",
            parameters: new Dictionary<string, object>()
            {
                { "UserId", userId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (user == null)
            throw new BadRequestException("The request invalid");

        return new OkResponseModel<UserProfileModel>(user);
    }

    public async Task<BaseResponseModel> CreateAsync(EditUserModel editUserModel, CancellationToken cancellationToken = default)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = editUserModel.Username,
            Fullname = editUserModel.Fullname,
            Email = editUserModel.Email,
            EmailConfirmed = true,
            PhoneNumber = editUserModel.PhoneNumber,
            PasswordHash = editUserModel.Password.HashMD5(),
            Address = editUserModel.Address,
            TotalAmountOwed = 0,
            Status = true,
            Created = DateTime.Now,
            IsDeleted = false
        };

        var duplicateUser = await _userRepository.CheckDuplicateAsync(user, cancellationToken).ConfigureAwait(false);
        if(!duplicateUser)
            throw new InvalidOperationException("User with the same name already exists.");
        
        if (editUserModel.Avatar != null)
        {
            user.Avatar = await editUserModel.Avatar.SaveImageAsync(_env);
        }

        var resultCreated = await _userRepository.CreateUserAsync(
                user:user,
                roles: editUserModel.RoleIds,
                cancellationToken:cancellationToken
            ).ConfigureAwait(false);
        if (!resultCreated)
            throw new InternalServerException("Create user fail");
        return new BaseResponseModel("Create user success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid userId, EditUserModel editUserModel, CancellationToken cancellationToken = default)
    {
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if(u == null)
            throw new BadRequestException("Invalid user id");

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = editUserModel.Username,
            Fullname = editUserModel.Fullname,
            Email = editUserModel.Email,
            EmailConfirmed = true,
            PhoneNumber = editUserModel.PhoneNumber,
            PasswordHash = editUserModel.Password.HashMD5(),
            Address = editUserModel.Address,
            TotalAmountOwed = 0,
            Status = true,
            Created = DateTime.Now,
            IsDeleted = false
        };
        
        if (editUserModel.Avatar != null)
        {
            await u.Avatar.DeleteImageAsync();
            user.Avatar = await editUserModel.Avatar.SaveImageAsync(_env);
        }

        var resultUpdated = await _userRepository.UpdateUserAsync(
                user:user, 
                roles: editUserModel.RoleIds,
                cancellationToken:cancellationToken
            ).ConfigureAwait(false);
        if (!resultUpdated)
            throw new InternalServerException("Update user fail");
        return new BaseResponseModel("Update user success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("Invalid user id");

        var resultDeleted = await _userRepository.DeleteUserAsync(userId, cancellationToken).ConfigureAwait(false);
        if(!resultDeleted)
            throw new InternalServerException("Deleted user fail");
        return new BaseResponseModel("Deleted user success");
    }

    public async Task<BaseResponseModel> AddRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("Invalid user id");

        var r = await _roleRepository.FindRoleByIdAsync(roleId, cancellationToken).ConfigureAwait(false);
        if(r == null)
            throw new BadRequestException("Invalid role id");

        var resultAdd = await _userRoleRepository.AddUserToRoleAsync(userId, roleId, cancellationToken).ConfigureAwait(false);
        if (!resultAdd)
            throw new InternalServerException("Add user to role fail");
        return new BaseResponseModel("Add user to role success");
    }

    public async Task<BaseResponseModel> AddRolesToUserAsync(Guid userId, List<Guid> roles,
        CancellationToken cancellationToken = default)
    {
        return new BaseResponseModel();
    }
    #endregion

    #region User Service (Member)

    public async Task<OkResponseModel<UserProfileModel>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        return await GetAsync(userId, cancellationToken).ConfigureAwait(false);
    }
    public async Task<BaseResponseModel> ChangePasswordAsync(ChangePasswordModel changePasswordModel, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextModel.Id);
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new BadRequestException("The request is invalid");

        if (u.PasswordHash != changePasswordModel.OldPassword.HashMD5())
            throw new BadRequestException("Old password is not correct");

        u.PasswordHash = changePasswordModel.NewPassword.HashMD5();

        var resultChange = await _userRepository.UpdateUserAsync(
                user: u, 
                roles: null ,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        if (!resultChange)
            throw new InternalServerException("Change password user fail");
        return new BaseResponseModel("Change password user success");
    }

    
    public async Task<BaseResponseModel> UpdateProfileAsync(EditProfileModel editProfileModel, CancellationToken cancellationToken = default)
    {
        var user = new User()
        {
            Fullname = editProfileModel.Fullname,
            Email = editProfileModel.Email,
            PhoneNumber = editProfileModel.PhoneNumber,
            Address = editProfileModel.Address,
            UserAddressId = editProfileModel.UserAddressId ?? default!
        };

        if (editProfileModel.Avatar != null)
        {
            user.Avatar = await editProfileModel.Avatar.SaveImageAsync(_env);
        }
        
        var resultUpdated = await _userRepository.UpdateUserAsync(
                user:user, 
                roles: null,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        if (!resultUpdated)
            throw new InternalServerException("Update user fail");
        return new BaseResponseModel("Update user success");
        
    }
    #endregion

    #region Private Service
    private UserContextModel GetUserContextModel(User user)
    {
        return new UserContextModel()
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Fullname = user.Fullname,
            Username = user.Username
        };
    }
    #endregion
}