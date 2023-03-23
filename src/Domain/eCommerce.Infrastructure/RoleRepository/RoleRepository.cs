using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.RoleRepository;

public class RoleRepository : IRoleRepository
{
    private readonly ILogger<RoleRepository> _logger;
    private readonly IDatabaseRepository _databaseRepository;
    private readonly string SQL_QUERY = "sp_Roles";
    public RoleRepository(ILogger<RoleRepository> logger, IDatabaseRepository databaseRepository)
    {
        _logger = logger;
        _databaseRepository = databaseRepository 
                              ?? throw new ArgumentNullException(nameof(databaseRepository));
    }
    public async Task<bool> CreateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        return await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "INSERT"},
                {"Id", Guid.NewGuid()},
                {"Name", role.Name},
                {"Description", role.Description}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        return await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "UPDATE"},
                {"Id", role.Id},
                {"Name", role.Name},
                {"Description", role.Description}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);

        return await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "DELETE"},
                { "Id", roleId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<IList<string>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        var roles = await _databaseRepository.GetAllAsync<Role>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ROLES_BY_USER_ID"},
                {"UserId", userId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!roles.NotNullOrEmpty())
            return default!;
            
        return roles.Select(x => x.Name).ToList();
    }

    public async Task<Role> FindRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);

        return await _databaseRepository.GetAsync<Role>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_ROLE_BY_ID"},
                {"Id", roleId}
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<Role> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleName);
        
        return await _databaseRepository.GetAsync<Role>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_ROLE_BY_NAME"},
                {"Name", roleName}
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> CheckDuplicateRole(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        
        var r = await _databaseRepository.GetAsync<Role>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "CHECK_DUPLICATE"},
                {"Name", role.Name }
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return r == null;
    }
}