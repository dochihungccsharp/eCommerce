using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Categories;
using eCommerce.Model.Paginations;

namespace eCommerce.Service.Categories;

public interface ICategoryService
{
    Task<OkResponseModel<PaginationModel<CategoryModel>>> GetAllAsync(CategoryFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<PaginationModel<CategoryModel>>> GetAllRootAsync(
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<CategoryModel>> GetAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<OkResponseModel<CategoryDetailsModel>> GetDetailsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateAsync(EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateAsync(Guid categoryId, EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ChangeStatusAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Category> FindByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<bool> CheckDuplicatedAsync(EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default);
    Task<bool> CheckAlreadyExistAsync(Guid categoryId, CancellationToken cancellationToken = default);
}