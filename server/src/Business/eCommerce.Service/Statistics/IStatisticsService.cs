using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Categories;
using eCommerce.Model.Orders;
using eCommerce.Model.Paginations;
using eCommerce.Model.Products;
using eCommerce.Model.Statistics;
using eCommerce.Model.Users;

namespace eCommerce.Service.Statistics;

public interface IStatisticsService
{
    Task<OkResponseModel<StatisticsModel>> GetMonthlyStatisticsAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<StatisticsModel>> GetYearlyStatisticsAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<IEnumerable<CategoryModel>>> GetTopCategoriesOfCurrentMonthAsync(int quantity, CancellationToken cancellationToken = default);
    Task<OkResponseModel<IEnumerable<UserModel>>> GetTopUsersOfCurrentMonthAsync(int quantity, CancellationToken cancellationToken = default);
    Task<OkResponseModel<IEnumerable<ProductModel>>> GetTopProductsOfCurrentMonthAsync(int quantity, CancellationToken cancellationToken = default);
    Task<OkResponseModel<IEnumerable<OrderDetailsModel>>> GetTopOrderOfCurrentMonthlyAsync(int quantity, CancellationToken cancellationToken = default);
}