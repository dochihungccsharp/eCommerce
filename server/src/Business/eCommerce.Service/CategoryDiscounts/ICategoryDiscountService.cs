using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CategoryDiscounts;
using eCommerce.Model.Paginations;

namespace eCommerce.Service.CategoryDiscounts;

public interface ICategoryDiscountService
{
    Task<OkResponseModel<PaginationModel<CategoryDiscountModel>>> GetAllAsync(CategoryDiscountFilterRequestModel filter,
        CancellationToken cancellationToken = default);

    Task<OkResponseModel<CategoryDiscountModel>> GetAsync(Guid categoryDiscountId, 
        CancellationToken cancellationToken = default);

    Task<OkResponseModel<CategoryDiscountDetailsModel>> GetDetailsAsync(Guid categoryDiscountId,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> CreateAsync(EditCategoryDiscountModel editCategoryDiscountModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> UpdateAsync(Guid categoryDiscountId, EditCategoryDiscountModel editCategoryDiscountModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> DeleteAsync(Guid categoryDiscountId, CancellationToken cancellationToken = default);


}