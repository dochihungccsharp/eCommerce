using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Categories;
using eCommerce.Model.Paginations;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using InvalidOperationException = System.InvalidOperationException;

namespace eCommerce.Service.Categories;

public class CategoryService : ICategoryService
{
    private const string SQL_QUERY = "sp_Categories";
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public CategoryService(IDatabaseRepository databaseRepository, IMapper mapper, IWebHostEnvironment env)
    {
        _databaseRepository = databaseRepository;
        _mapper = mapper;
        _env = env;
    }

    public async Task<OkResponseModel<PaginationModel<CategoryModel>>> GetAllAsync(CategoryFilterRequestModel filter,
        CancellationToken cancellationToken = default)
    {
        var categories = await _databaseRepository.PagingAllAsync<Category>(
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

        return new OkResponseModel<PaginationModel<CategoryModel>>(
            _mapper.Map<PaginationModel<CategoryModel>>(categories));
    }

    public async Task<OkResponseModel<PaginationModel<CategoryModel>>> GetAllRootAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _databaseRepository.GetAllAsync<CategoryModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL_ROOT"},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new OkResponseModel<PaginationModel<CategoryModel>>(
            _mapper.Map<PaginationModel<CategoryModel>>(categories));
    }

    public async Task<OkResponseModel<CategoryModel>> GetAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _databaseRepository.GetAsync<CategoryModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_BY_ID"},
                {"Id", categoryId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (category == null)
            throw new NotFoundException("The category is not found");
   
        return new OkResponseModel<CategoryModel>(category);
    }
    
    public async Task<OkResponseModel<CategoryDetailsModel>> GetDetailsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _databaseRepository.GetAsync<CategoryDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_DETAILS_BY_ID"},
                {"Id", categoryId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (category == null)
            throw new NotFoundException("The category is not found");
  
        return new OkResponseModel<CategoryDetailsModel>(category);
    }

    public async Task<BaseResponseModel> CreateAsync(EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default)
    {
        var duplicated = await CheckDuplicatedAsync(editCategoryModel, cancellationToken).ConfigureAwait(false);
        if (duplicated)
            throw new InvalidOperationException("Category with the same name already exists.");


        var targetPath = string.Empty;
        if (!string.IsNullOrEmpty(editCategoryModel.ImageUrl))
            targetPath = Path.Combine(_env.WebRootPath, "images", "categories", Path.GetFileName(editCategoryModel.ImageUrl));
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "INSERT"},
                {"Id", Guid.NewGuid()},
                {"Name", editCategoryModel.Name},
                {"Description", editCategoryModel.Description},
                {"ImageUrl", targetPath },
                {"ParentId", editCategoryModel.ParentId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        if (!string.IsNullOrEmpty(targetPath))
            await ImageExtensions.MoveFile(editCategoryModel.ImageUrl, targetPath);

        return new BaseResponseModel("Created category success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid categoryId, EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default)
    {
        // check for existence of category
        var c = await FindByIdAsync(categoryId, cancellationToken).ConfigureAwait(false);

        if (c == null) 
            throw new NotFoundException("The category is not found");
        
        // check for existence of category parents
        if (editCategoryModel.ParentId.HasValue)
        {
            var alreadyExistParent = await CheckAlreadyExistAsync(editCategoryModel.ParentId.Value, cancellationToken).ConfigureAwait(false);

            if (!alreadyExistParent)
                throw new BadRequestException("The category parents is not found");
        }
        
        // handle get path image
        var targetPath = string.Empty;
        if (!string.IsNullOrEmpty(editCategoryModel.ImageUrl))
        {
            if (string.IsNullOrEmpty(c.ImageUrl))
            {
                targetPath = Path.Combine(_env.WebRootPath, "images", "categories", Path.GetFileName(editCategoryModel.ImageUrl));
            }
            else if (c.ImageUrl != editCategoryModel.ImageUrl)
            {
                await c.ImageUrl.DeleteImageAsync();
                targetPath = Path.Combine(_env.WebRootPath, "images", "categories", Path.GetFileName(editCategoryModel.ImageUrl));
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(c.ImageUrl))
            {
                await c.ImageUrl.DeleteImageAsync();
            }
        }
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE"},
                { "Id", categoryId },
                { "Name", editCategoryModel.Name },
                { "Description", editCategoryModel.Description},
                { "ImageUrl", targetPath},
                { "ParentId", editCategoryModel.ParentId},
                { "Status", editCategoryModel.Status}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        if (!string.IsNullOrEmpty(targetPath))
            await ImageExtensions.MoveFile(editCategoryModel.ImageUrl, targetPath);
        
        return new BaseResponseModel("Update category success");
    }

    public async Task<BaseResponseModel> ChangeStatusAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var alreadyExist = await CheckAlreadyExistAsync(categoryId, cancellationToken).ConfigureAwait(false);

        if (!alreadyExist) 
            throw new NotFoundException("The category is not found");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "CHANGE_STATUS"},
                {"Id", categoryId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Change status category success");

    }

    public async Task<BaseResponseModel> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var c = await FindByIdAsync(categoryId, cancellationToken).ConfigureAwait(false);

        if (c == null) 
            throw new NotFoundException("The category is not found");

        if(!string.IsNullOrEmpty(c.ImageUrl))
            await c.ImageUrl.DeleteImageAsync();
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "DELETE"},
                {"Id", categoryId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Deleted category success");
    }

    // CheckDuplicated: if duplicated returns true, else returns false
    public async Task<bool> CheckDuplicatedAsync(EditCategoryModel editCategoryModel, CancellationToken cancellationToken = default)
    {
        var duplicatedCategory = await _databaseRepository.GetAsync<Category>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "CHECK_DUPLICATE"},
                {"Name", editCategoryModel.Name}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return duplicatedCategory != null;
    }

    // AlreadyExist: if already exist returns true, else returns false
    public async Task<bool> CheckAlreadyExistAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _databaseRepository.GetAsync<Category>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_BY_ID"},
                {"Id", categoryId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return category != null;
    }
    
    public async Task<Category> FindByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _databaseRepository.GetAsync<Category>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_BY_ID"},
                {"Id", categoryId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return category;
    }
}