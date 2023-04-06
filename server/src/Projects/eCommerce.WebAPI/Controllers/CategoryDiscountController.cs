using eCommerce.Model.CategoryDiscounts;
using eCommerce.Service.CategoryDiscounts;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class CategoryDiscountController : BaseController
{
    private readonly ICategoryDiscountService _categoryDiscountService;
    public CategoryDiscountController(
        ILogger<CategoryDiscountController> logger,
        ICategoryDiscountService categoryDiscountService
        ) : base(logger)
    {
        _categoryDiscountService = categoryDiscountService;
    }
    
    #region API Public
    [HttpGet]
    [Route("api/category-discounts")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery]CategoryDiscountFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryDiscountService.GetAllAsync(filter, cancellationToken)
            .ConfigureAwait(false));

    [HttpGet]
    [Route("api/category-discounts/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid categoryDiscountId, 
        CancellationToken cancellationToken = default)
        => Ok(await _categoryDiscountService.GetAsync(categoryDiscountId, cancellationToken).ConfigureAwait(false));

    [HttpGet]
    [Route("api/category-discounts/{id:guid}/details")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute(Name = "id")] Guid categoryDiscountId,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryDiscountService.GetDetailsAsync(categoryDiscountId, cancellationToken).ConfigureAwait(false));
    #endregion

    #region API Private

    [HttpPost]
    [Route("api/category-discounts")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditCategoryDiscountModel editCategoryDiscountModel,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryDiscountService.CreateAsync(editCategoryDiscountModel, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/category-discounts/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid categoryDiscountId,
        [FromBody] EditCategoryDiscountModel editCategoryDiscountModel, CancellationToken cancellationToken = default)
        => Ok(await _categoryDiscountService.UpdateAsync(categoryDiscountId, editCategoryDiscountModel, cancellationToken).ConfigureAwait(false));
    
    [HttpDelete]
    [Route("api/category-discounts/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid productId, 
        CancellationToken cancellationToken = default)
        => Ok(await _categoryDiscountService.DeleteAsync(productId, cancellationToken).ConfigureAwait(false));
    
    #endregion
}