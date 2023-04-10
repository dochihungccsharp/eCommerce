using System.Collections;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Categories;
using eCommerce.Model.Orders;
using eCommerce.Model.Products;
using eCommerce.Model.Statistics;
using eCommerce.Model.Users;

namespace eCommerce.Service.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly IDatabaseRepository _databaseRepository;
    private const string SQL_QUERY = "sp_StatisticsECommerce";

    public StatisticsService(IDatabaseRepository databaseRepository)
    {
        _databaseRepository = databaseRepository;
    }
    public async Task<OkResponseModel<StatisticsModel>> GetMonthlyStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var s = await _databaseRepository.GetAsync<StatisticsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_MONTHLY_STATISTICS" }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<StatisticsModel>(s);
    }

    public async Task<OkResponseModel<StatisticsModel>> GetYearlyStatisticsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OkResponseModel<IEnumerable<CategoryModel>>> GetTopCategoriesOfCurrentMonthAsync(int quantity, CancellationToken cancellationToken = default)
    {
        var cates = await _databaseRepository.GetAllAsync<CategoryModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_TOP_CATEGORIES_OF_CURRENT_MONTHLY" },
                { "Quantity", quantity }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IEnumerable<CategoryModel>>(cates);
    }

    public async Task<OkResponseModel<IEnumerable<UserModel>>> GetTopUsersOfCurrentMonthAsync(int quantity, CancellationToken cancellationToken = default)
    {
        var users = await _databaseRepository.GetAllAsync<UserModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_TOP_USERS_OF_CURRENT_MONTHLY" },
                { "Quantity", quantity }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IEnumerable<UserModel>>(users);
    }

    public async Task<OkResponseModel<IEnumerable<ProductModel>>> GetTopProductsOfCurrentMonthAsync(int quantity, CancellationToken cancellationToken = default)
    {
        var products = await _databaseRepository.GetAllAsync<ProductModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_TOP_PRODUCTS_OF_CURRENT_MONTHLY" },
                { "Quantity", quantity }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IEnumerable<ProductModel>>(products);
    }

    public async Task<OkResponseModel<IEnumerable<OrderDetailsModel>>> GetTopOrderOfCurrentMonthlyAsync(int quantity, CancellationToken cancellationToken = default)
    {
        var orders = await _databaseRepository.GetAllAsync<OrderDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_TOP_ORDERS_OF_CURRENT_MONTHLY" },
                { "Quantity", quantity }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IEnumerable<OrderDetailsModel>>(orders);
    }
}