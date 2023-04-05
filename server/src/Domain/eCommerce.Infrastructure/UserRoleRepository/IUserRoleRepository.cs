using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;

namespace eCommerce.Infrastructure.UserRoleRepository;

public interface IUserRoleRepository
{
    Task<bool> AddUserToRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken);
    Task<bool> IsUserInRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<bool> RemoveUserFromRole(Guid userId, Guid roleId, CancellationToken cancellationToken);
}