using eCommerce.Model.CategoryDiscounts;
using eCommerce.Model.Promotions;
using eCommerce.Service.Promotions;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class PromotionController : BaseController
{
    private readonly IPromotionService _promotionService;
    public PromotionController(ILogger<PromotionController> logger, IPromotionService promotionService) : base(logger)
    {
        _promotionService = promotionService;
    }
    
    #region API Public
    [HttpGet]
    [Route("api/promotions")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery]PromotionFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _promotionService.GetAllAsync(filter, cancellationToken)
            .ConfigureAwait(false));

    [HttpGet]
    [Route("api/promotions/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid promotionId, 
        CancellationToken cancellationToken = default)
        => Ok(await _promotionService.GetAsync(promotionId, cancellationToken).ConfigureAwait(false));
    
    #endregion

    #region API Private

    [HttpPost]
    [Route("api/promotions")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditPromotionModel editPromotionModel,
        CancellationToken cancellationToken = default)
        => Ok(await _promotionService.CreateAsync(editPromotionModel, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/promotions/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid promotionId,
        [FromBody] EditPromotionModel editPromotionModel, CancellationToken cancellationToken = default)
        => Ok(await _promotionService.UpdateAsync(promotionId, editPromotionModel, cancellationToken).ConfigureAwait(false));
    
    [HttpDelete]
    [Route("api/promotions/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid promotionId, 
        CancellationToken cancellationToken = default)
        => Ok(await _promotionService.DeleteAsync(promotionId, cancellationToken).ConfigureAwait(false));
    
    #endregion
}