using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.CategoryDiscounts;
using eCommerce.Model.Paginations;
using eCommerce.Model.Users;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;

namespace eCommerce.Service.CategoryDiscounts;

public class CategoryDiscountService: ICategoryDiscountService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IMapper _mapper;
    private const string SQL_QUERY = "sp_CategoryDiscount";
    
    public CategoryDiscountService(
        IDatabaseRepository databaseRepository,
        IMapper mapper)
    {
        _databaseRepository = databaseRepository;
        _mapper = mapper;
    }
    
    public async Task<OkResponseModel<PaginationModel<CategoryDiscountModel>>> GetAllAsync(CategoryDiscountFilterRequestModel filter, CancellationToken cancellationToken = default)
    {
        var categoryDiscounts = await _databaseRepository.PagingAllAsync<CategoryDiscount>(
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

        return new OkResponseModel<PaginationModel<CategoryDiscountModel>>(
            _mapper.Map<PaginationModel<CategoryDiscountModel>>(categoryDiscounts));
    }

    public async Task<OkResponseModel<CategoryDiscountModel>> GetAsync(Guid categoryDiscountId, CancellationToken cancellationToken = default)
    {
        var categoryDiscount = await _databaseRepository.GetAsync<CategoryDiscountModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", categoryDiscountId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (categoryDiscount == null)
            throw new NotFoundException("The category discount is not found");

        return new OkResponseModel<CategoryDiscountModel>(categoryDiscount);
    }

    public async Task<OkResponseModel<CategoryDiscountDetailsModel>> GetDetailsAsync(Guid categoryDiscountId, CancellationToken cancellationToken = default)
    {
        var categoryDiscount = await _databaseRepository.GetAsync<CategoryDiscountDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", categoryDiscountId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (categoryDiscount == null)
            throw new NotFoundException("The category discount details is not found");

        return new OkResponseModel<CategoryDiscountDetailsModel>(categoryDiscount);
    }

    public async Task<BaseResponseModel> CreateAsync(EditCategoryDiscountModel editCategoryDiscountModel, CancellationToken cancellationToken = default)
    {
        if(editCategoryDiscountModel.ProductExclusions != null)
            if (editCategoryDiscountModel.ProductExclusions.HasDuplicated(x => x.ProductId))
                return new BadRequestResponseModel("Product is duplicate.");
        
        // check data under data base
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "INSERT" },
                { "Id", Guid.NewGuid() },
                { "UserId", editCategoryDiscountModel.UserId },
                { "CategoryId", editCategoryDiscountModel.CategoryId },
                { "Code", editCategoryDiscountModel.Code },
                { "DiscountType", editCategoryDiscountModel.DiscountType },
                { "DiscountValue", editCategoryDiscountModel.DiscountValue },
                { "IsActive", editCategoryDiscountModel.IsActive },
                { "StartDate", editCategoryDiscountModel.StartDate },
                { "EndDate", editCategoryDiscountModel.EndDate },
                {"CategoryProductExclusions", editCategoryDiscountModel.ProductExclusions?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Create category discount success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid categoryDiscountId, EditCategoryDiscountModel editCategoryDiscountModel,
        CancellationToken cancellationToken = default)
    {
        
        if(editCategoryDiscountModel.ProductExclusions != null)
            if (editCategoryDiscountModel.ProductExclusions.HasDuplicated(x => x.ProductId))
                return new BadRequestResponseModel("Product is duplicate.");
        
        // check data under data base
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE" },
                { "Id", categoryDiscountId },
                { "UserId", editCategoryDiscountModel.UserId },
                { "CategoryId", editCategoryDiscountModel.CategoryId },
                { "Code", editCategoryDiscountModel.Code },
                { "DiscountType", editCategoryDiscountModel.DiscountType },
                { "DiscountValue", editCategoryDiscountModel.DiscountValue },
                { "IsActive", editCategoryDiscountModel.IsActive },
                { "StartDate", editCategoryDiscountModel.StartDate },
                { "EndDate", editCategoryDiscountModel.EndDate },
                {"CategoryProductExclusions", editCategoryDiscountModel.ProductExclusions?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return new BaseResponseModel("Update category discount success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid categoryDiscountId, CancellationToken cancellationToken = default)
    {
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "DELETE" },
                { "Id", categoryDiscountId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return new BaseResponseModel("Delete category discount success");
    }
}