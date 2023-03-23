using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;

namespace eCommerce.Infrastructure.RoleRepository;

public interface IRoleRepository
{
    Task<bool> CreateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IList<string>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Role> FindRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Role> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<bool> CheckDuplicateRole(Role role, CancellationToken cancellationToken = default);
}