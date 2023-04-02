using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CategoryDiscounts;
using eCommerce.Model.Paginations;
using eCommerce.Model.Promotions;
using eCommerce.Shared.Exceptions;

namespace eCommerce.Service.Promotions;

public class PromotionService : IPromotionService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IMapper _mapper;
    private const string SQL_QUERY = "sp_Promotions";
    
    public PromotionService(
        IDatabaseRepository databaseRepository,
        IMapper mapper)
    {
        _databaseRepository = databaseRepository;
        _mapper = mapper;
    }
    
    public async Task<OkResponseModel<PaginationModel<PromotionModel>>> GetAllAsync(PromotionFilterRequestModel filter, CancellationToken cancellationToken = default)
    {
        var promotions = await _databaseRepository.PagingAllAsync<Promotion>(
            sqlQuery: SQL_QUERY,
            pageIndex: filter.PageIndex,
            pageSize: filter.PageSize,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_ALL" },
                { "SearchString", filter.SearchString }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<PaginationModel<PromotionModel>>(
            _mapper.Map<PaginationModel<PromotionModel>>(promotions));
    }

    public async Task<OkResponseModel<PromotionDetailsModel>> GetAsync(Guid categoryDiscountId, CancellationToken cancellationToken = default)
    {
        var promotion = await _databaseRepository.GetAsync<PromotionDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", categoryDiscountId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (promotion == null)
            throw new NotFoundException("The promotion is not found");

        return new OkResponseModel<PromotionDetailsModel>(promotion);
    }

    public async Task<BaseResponseModel> CreateAsync(EditPromotionModel editPromotionModel, CancellationToken cancellationToken = default)
    {
        // check data under data base
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "INSERT" },
                { "Id", Guid.NewGuid() },
                { "UserId", editPromotionModel.UserId },
                { "Code", editPromotionModel.Code },
                { "DiscountType", editPromotionModel.DiscountType },
                { "DiscountValue", editPromotionModel.DiscountValue },
                { "MinimumOrderAmount", editPromotionModel.MinimumOrderAmount },
                { "IsActive", editPromotionModel.IsActive },
                { "StartDate", editPromotionModel.StartDate },
                { "EndDate", editPromotionModel.EndDate }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Create promotion success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid promotionId, EditPromotionModel editPromotionModel,
        CancellationToken cancellationToken = default)
    {
        // check data under data base
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE" },
                { "Id", promotionId },
                { "UserId", editPromotionModel.UserId },
                { "Code", editPromotionModel.Code },
                { "DiscountType", editPromotionModel.DiscountType },
                { "DiscountValue", editPromotionModel.DiscountValue },
                { "MinimumOrderAmount", editPromotionModel.MinimumOrderAmount },
                { "IsActive", editPromotionModel.IsActive },
                { "StartDate", editPromotionModel.StartDate },
                { "EndDate", editPromotionModel.EndDate },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return new BaseResponseModel("Update promotion success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid promotionId, CancellationToken cancellationToken = default)
    {
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "DELETE" },
                { "Id", promotionId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return new BaseResponseModel("Delete promotion success");
    }
}