using eCommerce.Service.Statistics;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class StatisticsController : BaseController
{
    private readonly IStatisticsService _statisticsService;
    public StatisticsController(ILogger<StatisticsController> logger, IStatisticsService statisticsService) : base(logger)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    [Route("api/statistics/monthly")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetMonthlyStatisticsAsync(CancellationToken cancellationToken = default)
        => Ok(await _statisticsService.GetMonthlyStatisticsAsync(cancellationToken).ConfigureAwait(false));
    
    
    [HttpGet]
    [Route("api/statistics/categories/top-monthly")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetTopCategoriesOfCurrentMonthAsync([FromQuery]int quantity = 10, CancellationToken cancellationToken = default)
        => Ok(await _statisticsService.GetTopCategoriesOfCurrentMonthAsync(quantity, cancellationToken).ConfigureAwait(false));

        [HttpGet]
    [Route("api/statistics/products/top-monthly")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetTopProductsOfCurrentMonthAsync([FromQuery]int quantity = 10, CancellationToken cancellationToken = default)
        => Ok(await _statisticsService.GetTopProductsOfCurrentMonthAsync(quantity, cancellationToken).ConfigureAwait(false));

    
    [HttpGet]
    [Route("api/statistics/users/top-monthly")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetTopUsersOfCurrentMonthAsync([FromQuery]int quantity = 10, CancellationToken cancellationToken = default)
        => Ok(await _statisticsService.GetTopUsersOfCurrentMonthAsync(quantity, cancellationToken).ConfigureAwait(false));

    [HttpGet]
    [Route("api/statistics/orders/top-monthly")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> GetTopOrderOfCurrentMonthlyAsync([FromQuery]int quantity = 10, CancellationToken cancellationToken = default)
        => Ok(await _statisticsService.GetTopOrderOfCurrentMonthlyAsync(quantity, cancellationToken).ConfigureAwait(false));

}