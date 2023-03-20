using System.Data;
using System.Data.SqlClient;
using eCommerce.Domain.Abstractions.Paginations;
using eCommerce.Shared.Configurations;
using eCommerce.Shared.Extensions;
using Microsoft.Extensions.Configuration;

namespace eCommerce.Infrastructure.DatabaseRepository;

public class DatabaseRepository : IDatabaseRepository
{
    private readonly DatabaseSetting _databaseSetting;

    public DatabaseRepository(IConfiguration configuration)
    {
        _databaseSetting = configuration?.GetOptions<DatabaseSetting>()
                           ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default) where T : class, new()
        => await GetAllAsync<T>(sqlQuery, commandType, null, cancellationToken).ConfigureAwait(false);

    public async Task<IEnumerable<T>> GetAllAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default) where T : class, new()
    {
        using SqlConnection sqlConnection = new(_databaseSetting.Default);
        using SqlCommand sqlCommand = new(sqlQuery, sqlConnection)
        {
            CommandText = sqlQuery,
            CommandType = commandType
        };

        if (parameters.NotNullOrEmpty())
        {
            foreach (var param in parameters)
                sqlCommand.Parameters.AddWithValue(param.Key, param.Value);
        }

        await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using SqlDataAdapter sqlDataAdapter = new(sqlCommand);
        DataTable dataTable = new();
        sqlDataAdapter.Fill(dataTable);

        await sqlConnection.CloseAsync().ConfigureAwait(false);

        return dataTable.ToList<T>();
    }

    public async Task<IPagedList<T>> PagingAllAsync<T>(string sqlQuery, int pageIndex, int pageSize,
        CommandType commandType = CommandType.StoredProcedure, CancellationToken cancellationToken = default)
        where T : class, new()
        => await PagingAllAsync<T>(sqlQuery, pageIndex, pageSize, commandType, cancellationToken).ConfigureAwait(false);

    public async Task<IPagedList<T>> PagingAllAsync<T>(string sqlQuery, int pageIndex, int pageSize,
        CommandType commandType = CommandType.StoredProcedure, Dictionary<string, object> parameters = null,
        CancellationToken cancellationToken = default) where T : class, new()
    {
        using SqlConnection sqlConnection = new(_databaseSetting.Default);
        using SqlCommand sqlCommand = new(sqlQuery, sqlConnection)
        {
            CommandText = sqlQuery,
            CommandType = commandType
        };

        if (parameters.NotNullOrEmpty())
        {
            foreach (var param in parameters)
                sqlCommand.Parameters.AddWithValue(param.Key, param.Value);

            sqlCommand.Parameters.AddWithValue("PageIndex", pageIndex);
            sqlCommand.Parameters.AddWithValue("PageSize", pageSize);
        }

        await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using SqlDataAdapter sqlDataAdapter = new(sqlCommand);
        DataTable dataTable = new();
        sqlDataAdapter.Fill(dataTable);

        await sqlConnection.CloseAsync().ConfigureAwait(false);
        return dataTable.ToList<T>().ToPageList<T>(pageIndex, pageSize);
    }

    public async Task<T> GetAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default) where T : class, new()
        => await GetAsync<T>(sqlQuery, commandType, cancellationToken).ConfigureAwait(false);

    public async Task<T> GetAsync<T>(string sqlQuery, CommandType commandType = CommandType.StoredProcedure, Dictionary<string, object> parameters = null,
        CancellationToken cancellationToken = default) where T : class, new()
    {
        var items = await GetAllAsync<T>(sqlQuery, commandType, parameters, cancellationToken).ConfigureAwait(false);
        if (items.NotNullOrEmpty())
            return items.FirstOrDefault();

        return default;
    }

    public async Task<bool> ExecuteAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default)
        => await ExecuteAsync(sqlQuery, commandType, cancellationToken).ConfigureAwait(false);

    public async Task<bool> ExecuteAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure, Dictionary<string, object> parameters = null,
        CancellationToken cancellationToken = default)
    {
        using SqlConnection sqlConnection = new(_databaseSetting.Default);
        using SqlCommand sqlCommand = new(sqlQuery, sqlConnection)
        {
            CommandText = sqlQuery,
            CommandType = commandType
        };

        if (parameters.NotNullOrEmpty())
        {
            foreach (var param in parameters)
                if (param.Value is DataTable)
                {
                    sqlCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = param.Key,
                        SqlDbType = SqlDbType.Structured,
                        TypeName = $"dbo.{param.Key}TableType",
                        Value = param.Value
                    });
                }
                else
                {
                    sqlCommand.Parameters.AddWithValue(param.Key, param.Value);
                }
        }

        await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

        int changedRows = await sqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        await sqlConnection.CloseAsync().ConfigureAwait(false);

        return changedRows > 0;
    }

    public async Task<object> ExecuteScalarAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default)
        => await ExecuteScalarAsync(sqlQuery, commandType, cancellationToken).ConfigureAwait(false);
    
    public async Task<object> ExecuteScalarAsync(string sqlQuery, CommandType commandType = CommandType.StoredProcedure,
        Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection sqlConnection = new(_databaseSetting.Default);
        using SqlCommand sqlCommand = new(sqlQuery, sqlConnection)
        {
            CommandText = sqlQuery,
            CommandType = commandType
        };

        if (parameters.NotNullOrEmpty())
        {
            foreach (var param in parameters)
                sqlCommand.Parameters.AddWithValue(param.Key, param.Value);
        }

        await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

        object result = await sqlCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

        await sqlConnection.CloseAsync().ConfigureAwait(false);

        return result;
    }
    
}