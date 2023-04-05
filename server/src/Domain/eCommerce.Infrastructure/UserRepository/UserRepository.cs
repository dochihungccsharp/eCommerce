using System.Data;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Roles;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDatabaseRepository _databaseRepository;
    private readonly string SQL_QUERY = "sp_Users";

    public UserRepository(ILogger<UserRepository> logger, IDatabaseRepository databaseRepository)
    {
        _logger = logger;
        _databaseRepository = databaseRepository 
                              ?? throw new ArgumentNullException(nameof(databaseRepository));
    }
    public async Task<bool> CreateUserAsync(User user, List<AddRoleModel> roles = null,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "INSERT"},
                {"Id", user.Id == null ? Guid.NewGuid() : user.Id},
                {"Username", user.Username},
                {"Fullname", user.Fullname},
                {"Email", user.Email},
                {"EmailConfirmed", user.EmailConfirmed},
                {"PasswordHash", user.PasswordHash},
                {"PhoneNumber", user.PhoneNumber},
                {"Avatar", user.Avatar},
                {"Address", user.Address},
                {"TotalAmountOwed", user.TotalAmountOwed},
                {"UserAddressId", user.UserAddressId},
                {"Roles", roles?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "DELETE"},
                {"Id", userId},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> UpdateUserAsync(User user, List<AddRoleModel> roles = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "UPDATE"},
                {"Id", user.Id},
                {"Username", user.Username},
                {"Fullname", user.Fullname},
                {"Email", user.Email},
                {"EmailConfirmed", user.EmailConfirmed},
                {"PasswordHash", user.PasswordHash},
                {"PhoneNumber", user.PhoneNumber},
                {"Avatar", user.Avatar},
                {"Address", user.Address},
                {"UserAddressId", user.UserAddressId},
                {"Roles", roles?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

    }

    public async Task<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        return await _databaseRepository.GetAsync<User>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_BY_EMAIL"},
                {"Email", email}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<User> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        return await _databaseRepository.GetAsync<User>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_BY_ID"},
                {"Id", userId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<User> FindUserByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userName);
        
        return await _databaseRepository.GetAsync<User>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            { 
                {"Activity", "FIND_BY_NAME"},
                {"Username", userName}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> CheckDuplicateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        var u = await _databaseRepository.GetAsync<User>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "CHECK_DUPLICATE"},
                {"Id", user.Id},
                {"Username", user.Username},
                {"Email", user.Email},
                {"PhoneNumber", user.PhoneNumber}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return u == null;
    }
}