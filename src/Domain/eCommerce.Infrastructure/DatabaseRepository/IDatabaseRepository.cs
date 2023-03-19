using System.Data;

namespace eCommerce.Infrastructure.DatabaseRepository;

public interface IDatabaseRepository
{
    Task<IEnumerable<T>> GetAllAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default) where T : class, new();
    
    Task<IEnumerable<T>> GetAllAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        where T : class, new();
}