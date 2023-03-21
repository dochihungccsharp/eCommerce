using eCommerce.Infrastructure.RoleRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Shared.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace eCommerce.Service.Cache.RoleCache;

public class RoleCacheService : IRoleCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public RoleCacheService(IMemoryCache cache, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _cache = cache;
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }
    
    public async Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var u = await _userRepository.FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (u == null)
            throw new NotFoundException("The user is not found");
        
        var cacheKey = $"UserRoles-{userId}";
        if (_cache.TryGetValue(cacheKey, out IList<string> userRoles))
        {
            return userRoles;
        }

        userRoles = await _roleRepository.GetRolesByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        _cache.Set(cacheKey, userRoles, cacheOptions);

        return userRoles;
    }
}