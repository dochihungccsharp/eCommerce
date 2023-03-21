using System.Data;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDatabaseRepository _databaseRepository;

    public UserRepository(ILogger<UserRepository> logger, IDatabaseRepository databaseRepository)
    {
        _logger = logger;
        _databaseRepository = databaseRepository 
                              ?? throw new ArgumentNullException(nameof(databaseRepository));
    }
    public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_InsertUser",
            parameters: new Dictionary<string, object>()
            {
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
                {"Created", user.Created},
                {"IsDeleted", user.IsDeleted}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_DeleteUser",
            parameters: new Dictionary<string, object>()
            {
                {"Id", Guid.NewGuid()},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_UpdateUser",
            parameters: new Dictionary<string, object>()
            {
                {"Id", user.Id},
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
                {"IsDeleted", user.IsDeleted}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

    }

    public async Task<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        return await _databaseRepository.GetAsync<User>(
            sqlQuery: "sp_FindByEmail",
            parameters: new Dictionary<string, object>()
            {
                {"Email", email}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<User> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        return await _databaseRepository.GetAsync<User>(
            sqlQuery: "sp_FindById",
            parameters: new Dictionary<string, object>()
            {
                {"Id", userId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<User> FindUserByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userName);
        
        return await _databaseRepository.GetAsync<User>(
            sqlQuery: "sp_FindByName",
            parameters: new Dictionary<string, object>()
            {
                {"Username", userName}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> CheckDuplicateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        var u = await _databaseRepository.GetAsync<User>(
            sqlQuery: "sp_CheckDuplicateUser",
            parameters: new Dictionary<string, object>()
            {
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