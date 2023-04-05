using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.UserRoleRepository;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly ILogger<UserRoleRepository> _logger;
    private readonly IDatabaseRepository _databaseRepository;
    public UserRoleRepository(ILogger<UserRoleRepository> logger, IDatabaseRepository databaseRepository)
    {
        _logger = logger;
        _databaseRepository = databaseRepository 
                              ?? throw new ArgumentNullException(nameof(databaseRepository));
    }
    public async Task<bool> AddUserToRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        
        return  await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_AddUserToRole",
            parameters: new Dictionary<string, object>()
            {
                { "UserId", userId },
                { "RoleId", roleId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleName);
        return (await _databaseRepository.GetAllAsync<User>(
            sqlQuery: "sp_GetUsersInRoleByRoleName",
            parameters: new Dictionary<string, object>()
            {
                { "RoleName", roleName }
            }, 
            cancellationToken: cancellationToken
        ).ConfigureAwait(false)).ToList();
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        
        return (int)(await _databaseRepository.ExecuteScalarAsync(
            sqlQuery: "sp_IsUserInRole",
            parameters: new Dictionary<string, object>()
            {
                {"UserId", userId},
                {"RoleId", roleId}
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false)) > 0;
    }

    public async Task<bool> RemoveUserFromRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_RemoveUserFromRole",
            parameters: new Dictionary<string, object>()
            {
                {"UserId", userId},
                {"RoleId", roleId}
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false);

    }
}