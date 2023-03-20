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
            sqlQuery: "sp_InsertRole",
            parameters: new Dictionary<string, object>()
            {
                {"Id", Guid.NewGuid()},
                {"Username", role.Name},
                {"Description", role.Description},
                {"Created", DateTime.Now}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        return await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_UpdateRole",
            parameters: new Dictionary<string, object>()
            {
                {"Id", role.Id == null ? Guid.NewGuid() : role.Id},
                {"Username", role.Name},
                {"Description", role.Description}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);

        return await _databaseRepository.ExecuteAsync(
            sqlQuery: "sp_DeleteRole",
            parameters: new Dictionary<string, object>()
            {
                { "Id", roleId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<IList<string>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        var roles = await _databaseRepository.GetAllAsync<Role>(
            sqlQuery: "sp_GetUserRolesByUserId",
            parameters: new Dictionary<string, object>()
            {
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
            sqlQuery: "sp_FindRoleById",
            parameters: new Dictionary<string, object>()
            {
                {"Id", roleId}
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task<Role> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleName);
        
        return await _databaseRepository.GetAsync<Role>(
            sqlQuery: "sp_FindRoleById",
            parameters: new Dictionary<string, object>()
            {
                {"RoleName", roleName}
            }, cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}