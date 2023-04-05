using System.Data;
using eCommerce.Domain.Abstractions.Paginations;

namespace eCommerce.Infrastructure.DatabaseRepository;

public interface IDatabaseRepository
{
    Task<IEnumerable<T>> GetAllAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default) where T : class, new();
    
    Task<IEnumerable<T>> GetAllAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        where T : class, new();

    Task<IPagedList<T>> PagingAllAsync<T>(string sqlQuery, int pageIndex, int pageSize,
        CommandType commandType = CommandType.StoredProcedure, 
        CancellationToken cancellationToken = default) where T : class, new();
    
    Task<IPagedList<T>> PagingAllAsync<T>(string sqlQuery, int pageIndex, int pageSize,
        CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        where T : class, new();
    
    Task<T> GetAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default) 
        where T : class, new();
    
    Task<T> GetAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        where T : class, new();
    
    Task<bool> ExecuteAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default);
    
    Task<bool> ExecuteAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
    
    Task<object> ExecuteScalarAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default);
    
    Task<object> ExecuteScalarAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
    
}