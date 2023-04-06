using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Brands;
using eCommerce.Model.Paginations;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using InvalidOperationException = System.InvalidOperationException;

namespace eCommerce.Service.Brands;

public class BrandService : IBrandService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private const string SQL_QUERY = "sp_Brands";
    
    public BrandService(IDatabaseRepository databaseRepository, IMapper mapper, IWebHostEnvironment env)
    {
        _databaseRepository = databaseRepository;
        _mapper = mapper;
        _env = env;
    }
    
    public async Task<OkResponseModel<PaginationModel<BrandModel>>> GetAllAsync(BrandFilterRequestModel filter, CancellationToken cancellationToken = default)
    {
        var brands = await _databaseRepository.PagingAllAsync<Brand>(
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

        return new OkResponseModel<PaginationModel<BrandModel>>(_mapper.Map<PaginationModel<BrandModel>>(brands));
    }

    public async Task<OkResponseModel<BrandModel>> GetAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var brand = await _databaseRepository.GetAsync<BrandModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", brandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (brand == null)
            throw new NotFoundException("The brand is not found");

        return new OkResponseModel<BrandModel>(brand);
    }

    public async Task<OkResponseModel<BrandDetailsModel>> GetDetailsAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var brand  = await _databaseRepository.GetAsync<BrandDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_DETAILS_BY_ID" },
                { "Id", brandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
            
        if (brand == null)
            throw new NotFoundException("The brand is not found");

        return new OkResponseModel<BrandDetailsModel>(brand);
    }
    
    
    public async Task<BaseResponseModel> CreateAsync(EditBrandModel editBrandModel, CancellationToken cancellationToken)
    {
        var checkDuplicated = await CheckDuplicatedAsync(editBrandModel, cancellationToken).ConfigureAwait(false);
        if (checkDuplicated)
            throw new InvalidOperationException("Brand with the same name already exits.");

        var targetPath = string.Empty;
        if (!string.IsNullOrEmpty(editBrandModel.LogoURL))
            targetPath = Path.Combine(_env.WebRootPath, "images","brands", Path.GetFileName(editBrandModel.LogoURL));


        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "INSERT" },
                { "Id", Guid.NewGuid() },
                { "Name", editBrandModel.Name },
                { "LogoURL",  targetPath },
                { "Description", editBrandModel.Description }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(targetPath))
            await ImageExtensions.MoveFile(editBrandModel.LogoURL, targetPath);

      
        return new BaseResponseModel("Create brand success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid brandId, EditBrandModel editBrandModel, CancellationToken cancellationToken)
    {
        var b = await FindByIdAsync(brandId, cancellationToken).ConfigureAwait(false);
        if(b == null)
            throw new NotFoundException("The brand is not found");
        
        // handle get path image
        var targetPath = string.Empty;
        if (!string.IsNullOrEmpty(editBrandModel.LogoURL)) // có ảnh gửi lên
        {
            if (string.IsNullOrEmpty(b.LogoURL)) // db không có ảnh
            {
                targetPath = Path.Combine(_env.WebRootPath, "images", "brands", Path.GetFileName(editBrandModel.LogoURL));
            }
            else if (b.LogoURL != editBrandModel.LogoURL) // db có ảnh , != ảnh mới gửi lên
            {
                await b.LogoURL.DeleteImageAsync();
                targetPath = Path.Combine(_env.WebRootPath, "images","brands", Path.GetFileName(editBrandModel.LogoURL));
            }
        }
        else // không có ảnh gửi lên
        {
            // db có ảnh
            if (!string.IsNullOrEmpty(b.LogoURL))
            {
                await b.LogoURL.DeleteImageAsync();
            }
        }
        
        // update brand
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE" },
                { "Id", brandId },
                { "Name", editBrandModel.Name },
                { "LogoURL", targetPath },
                { "Status", editBrandModel.Status }
            }, 
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(targetPath))
            await ImageExtensions.MoveFile(editBrandModel.LogoURL, targetPath);
 
        return new BaseResponseModel("Update brand success");
    }

    public async Task<BaseResponseModel> ChangeStatusAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var checkAlreadyExist = await CheckAlreadyExistAsync(brandId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExist)
            throw new NotFoundException("The brand is not found");
            
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CHANGE_STATUS" },
                { "Id", brandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Change status success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var b = await FindByIdAsync(brandId, cancellationToken);
        if (b == null)
            throw new NotFoundException("The brand is not found");

        if (!string.IsNullOrEmpty(b.LogoURL))
            await b.LogoURL.DeleteImageAsync();
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "DELETE"},
                {"Id", brandId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Deleted brand success");
    }
    
    // CheckDuplicated: if duplicated returns true, else returns false
    public async Task<bool> CheckDuplicatedAsync(EditBrandModel editBrandModel, CancellationToken cancellationToken = default)
    {
        var duplicatedBrand = await _databaseRepository.GetAsync<Brand>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CHECK_DUPLICATE" },
                { "Name", editBrandModel.Name }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
            
        return duplicatedBrand != null;
    }

    // AlreadyExist: if already exist returns true, else returns false
    public async Task<bool> CheckAlreadyExistAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var brand = await _databaseRepository.GetAsync<Brand>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", brandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return brand != null;
    }
    
    // Find Brand By Id
    public async Task<Brand> FindByIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var brand = await _databaseRepository.GetAsync<Brand>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", brandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return brand;
    }
}