using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;

namespace eCommerce.Infrastructure.UserRoleRepository;

public class UserRoleRepository : IUserRoleRepository
{
    public Task<bool> AddUserToRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        throw new NotImplementedException();
    }

    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roleName);
        throw new NotImplementedException();
    }

    public Task<bool> IsUserInRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        throw new NotImplementedException();
    }

    public Task<bool> RemoveUserFromRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        throw new NotImplementedException();
    }
}