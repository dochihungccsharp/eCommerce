namespace eCommerce.Service.Cache.RoleCache;

public interface IRoleCacheService
{
    Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}