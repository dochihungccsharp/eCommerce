using eCommerce.Domain.Domains;
using eCommerce.Model.Roles;

namespace eCommerce.Infrastructure.UserRepository;

public interface IUserRepository
{
    Task<bool> CreateUserAsync(User user, List<AddRoleModel> roles = null, CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
        
    Task<bool> UpdateUserAsync(User user, List<AddRoleModel> roles = null, CancellationToken cancellationToken = default);
    
    Task<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User> FindUserByNameAsync(string userName, CancellationToken cancellationToken = default);
    
    Task<bool> CheckDuplicateAsync(User user, CancellationToken cancellationToken = default);
}