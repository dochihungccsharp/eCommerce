using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CategoryDiscounts;
using eCommerce.Model.Paginations;
using eCommerce.Model.Promotions;

namespace eCommerce.Service.Promotions;

public interface IPromotionService
{
    Task<OkResponseModel<PaginationModel<PromotionModel>>> GetAllAsync(PromotionFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<PromotionDetailsModel>> GetAsync(Guid categoryDiscountId,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> CreateAsync(EditPromotionModel editPromotionModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> UpdateAsync(Guid promotionId, EditPromotionModel editPromotionModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> DeleteAsync(Guid promotionId, CancellationToken cancellationToken = default);
}