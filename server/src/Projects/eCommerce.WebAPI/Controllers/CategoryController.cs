using eCommerce.Model.Categories;
using eCommerce.Service.Categories;
using eCommerce.Shared.Consts;
using eCommerce.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

[ApiController]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService) : base(logger)
    {
        _categoryService = categoryService;
    }

    #region API Public
    [HttpGet]
    [Route("api/categories")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] CategoryFilterRequestModel filter,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.GetAllAsync(filter, cancellationToken)
            .ConfigureAwait(false));
    
    [HttpGet]
    [Route("api/categories/root")]
    public async Task<IActionResult> GetAllRootAsync(
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.GetAllRootAsync(cancellationToken)
            .ConfigureAwait(false));
    
    [HttpGet]
    [Route("api/categories/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute(Name = "id")] Guid categoryId,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.GetAsync(categoryId, cancellationToken).ConfigureAwait(false));

    [HttpGet]
    [Route("api/categories/{id:guid}/details")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute(Name = "id")] Guid categoryId,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.GetDetailsAsync(categoryId, cancellationToken).ConfigureAwait(false));
    
    #endregion

    #region API Private

    [HttpPost]
    [Route("api/categories")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> CreateAsync([FromBody] EditCategoryModel editCategoryModel,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.CreateAsync(editCategoryModel, cancellationToken).ConfigureAwait(false));

    [HttpPut]
    [Route("api/categories/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> UpdateAsync([FromRoute(Name = "id")] Guid categoryId,
        [FromBody] EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default)
        => Ok(await _categoryService.UpdateAsync(categoryId, editCategoryModel, cancellationToken)
            .ConfigureAwait(false));


    [HttpDelete]
    [Route("api/categories/{id:guid}")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] Guid categoryId,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.DeleteAsync(categoryId, cancellationToken).ConfigureAwait(false));
    
    [HttpPut]
    [Route("api/categories/{id:guid}/status")]
    [Authorize(Roles.Admin)]
    public async Task<IActionResult> ChangeStatusAsync([FromRoute(Name = "id")] Guid categoryId,
        CancellationToken cancellationToken = default)
        => Ok(await _categoryService.ChangeStatusAsync(categoryId, cancellationToken).ConfigureAwait(false));
    #endregion

}