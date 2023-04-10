using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Paginations;
using eCommerce.Model.Products;
using eCommerce.Service.Brands;
using eCommerce.Service.Categories;
using eCommerce.Service.Suppliers;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using InvalidOperationException = System.InvalidOperationException;

namespace eCommerce.Service.Products;


public class ProductService : IProductService
{
    private const string SQL_QUERY = "sp_Products";
    private readonly IDatabaseRepository _databaseRepository;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly ISupplierService _supplierService;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public ProductService(
        IDatabaseRepository databaseRepository,
        ICategoryService categoryService,
        IBrandService brandService,
        ISupplierService supplierService,
        IMapper mapper,
        IWebHostEnvironment env
    )
    {
        _databaseRepository = databaseRepository;
        _categoryService = categoryService;
        _brandService = brandService;
        _supplierService = supplierService;
        _mapper = mapper;
        _env = env;
    }

    public async Task<OkResponseModel<PaginationModel<ProductModel>>> GetAllAsync(
        ProductFilterRequestModel filter, CancellationToken cancellationToken = default)
    {
        var products = await _databaseRepository.PagingAllAsync<Product>(
            sqlQuery: SQL_QUERY,
            pageIndex: filter.PageIndex,
            pageSize: filter.PageSize,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_ALL" },
                { "SearchString", filter.SearchString },
                { "CategoryId", filter.CategoryId },
                { "BrandId", filter.BrandId },
                { "SupplierId", filter.SupplierId },
                { "FromTime", filter.FromTime },
                { "ToTime", filter.ToTime },
                { "FromPrice", filter.FromPrice },
                { "ToPrice", filter.ToPrice },
                { "IsBestSelling", filter.IsBestSelling },
                { "IsNew", filter.IsNew},
                { "Status", filter.Status},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<PaginationModel<ProductModel>>(
            _mapper.Map<PaginationModel<ProductModel>>(products));
    }

    public async Task<OkResponseModel<ProductModel>> GetAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _databaseRepository.GetAsync<ProductModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (product == null)
            throw new NotFoundException("The product is not found");

        return new OkResponseModel<ProductModel>(product);
    }

    public async Task<OkResponseModel<ProductDetailsModel>> GetDetailsAsync(Guid productId,
        CancellationToken cancellationToken)
    {
        var productDetails = await _databaseRepository.GetAsync<ProductDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_DETAILS_BY_ID" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (productDetails == null)
            throw new NotFoundException("The product is not found");

        return new OkResponseModel<ProductDetailsModel>(productDetails);
    }

    public async Task<BaseResponseModel> CreateAsync(EditProductModel editProductModel,
        CancellationToken cancellationToken = default)
    {
        // check duplicate
        var checkDuplicatedProduct = await CheckDuplicatedAsync(editProductModel, cancellationToken).ConfigureAwait(false);
        if (checkDuplicatedProduct)
            throw new InvalidOperationException("Product with the same name already exists.");
        
        // check is already exist category
        var checkAlreadyExistCategory = await _categoryService
            .CheckAlreadyExistAsync(editProductModel.CategoryId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExistCategory)
            throw new NotFoundException("The category is not found");

        // check is already exist supplier
        if (editProductModel.SupplierId.HasValue)
        {
            var checkAlreadyExistSupplier = await _supplierService
                .CheckAlreadyExistAsync(editProductModel.SupplierId.Value, cancellationToken).ConfigureAwait(false);
            if (!checkAlreadyExistSupplier)
                throw new NotFoundException("The supplier is not found");
        }

        // check is already exist brand
        if (editProductModel.BrandId.HasValue)
        {
            var checkAlreadyExistBrand = await _brandService
                .CheckAlreadyExistAsync(editProductModel.BrandId.Value, cancellationToken).ConfigureAwait(false);
            if (!checkAlreadyExistBrand)
                throw new NotFoundException("The brand is not found");
        }
        

        // handler image product
        var targetPath = string.Empty;
        if (!string.IsNullOrEmpty(editProductModel.ImageUrl))
            targetPath = Path.Combine(_env.WebRootPath, "images", "products", Path.GetFileName(editProductModel.ImageUrl));
        
        var slug = string.Empty;
        if (string.IsNullOrEmpty(editProductModel.Slug))
            slug = editProductModel.Name.ConvertToSlug();
        else
            slug = editProductModel.Slug.ConvertToSlug();
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "INSERT" },
                { "Id", Guid.NewGuid() },
                { "Name", editProductModel.Name },
                { "Slug", slug },
                { "Description", editProductModel.Description },
                { "ImageUrl", targetPath },
                { "OriginalPrice", editProductModel.OriginalPrice },
                { "Price", editProductModel.Price },
                { "CategoryId", editProductModel.CategoryId },
                { "SupplierId", editProductModel.SupplierId },
                { "BrandId", editProductModel.BrandId },
                { "Status", editProductModel.Status },
                { "IsBestSelling", editProductModel.IsBestSelling },
                { "IsNew", editProductModel.IsNew },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if(!string.IsNullOrEmpty(targetPath))
            await ImageExtensions.MoveFile(editProductModel.ImageUrl, targetPath);

        return new BaseResponseModel("Create product success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid productId, EditProductModel editProductModel,
        CancellationToken cancellationToken = default)
    {
        // check is already exist
        var p = await FindByIdAsync(productId, cancellationToken).ConfigureAwait(false);
        if (p == null)
            throw new NotFoundException("The product is not found");

        // check is already exist category
        var checkAlreadyExistCategory = await _categoryService
            .CheckAlreadyExistAsync(editProductModel.CategoryId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExistCategory)
            throw new NotFoundException("The category is not found");

        // check is already exist supplier
        if (editProductModel.SupplierId.HasValue)
        {
            var checkAlreadyExistSupplier = await _supplierService
                .CheckAlreadyExistAsync(editProductModel.SupplierId.Value, cancellationToken).ConfigureAwait(false);
            if (!checkAlreadyExistSupplier)
                throw new NotFoundException("The supplier is not found");
        }

        // check is already exist brand
        if (editProductModel.BrandId.HasValue)
        {
            var checkAlreadyExistBrand = await _brandService
                .CheckAlreadyExistAsync(editProductModel.BrandId.Value, cancellationToken).ConfigureAwait(false);
            if (!checkAlreadyExistBrand)
                throw new NotFoundException("The brand is not found");
        }

        // handler image product
        var targetPath = string.Empty;
        if (!string.IsNullOrEmpty(editProductModel.ImageUrl)) 
        {
            if (string.IsNullOrEmpty(p.ImageUrl))
            {
                targetPath = Path.Combine(_env.WebRootPath, "images", "products", Path.GetFileName(editProductModel.ImageUrl));
            }
            else if (p.ImageUrl != editProductModel.ImageUrl)
            {
                await p.ImageUrl.DeleteImageAsync();
                targetPath = Path.Combine(_env.WebRootPath, "images", "products", Path.GetFileName(editProductModel.ImageUrl));
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(p.ImageUrl))
            {
                await p.ImageUrl.DeleteImageAsync();
            }
        }

        string slug = string.Empty;
        if (string.IsNullOrEmpty(editProductModel.Slug))
            slug = editProductModel.Name.ConvertToSlug();
        else
            slug = editProductModel.Slug.ConvertToSlug();

        // update
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE" },
                { "Id", productId },
                { "Name", editProductModel.Name },
                { "Slug", slug },
                { "Description", editProductModel.Description },
                { "ImageUrl", targetPath },
                { "OriginalPrice", editProductModel.OriginalPrice },
                { "Price", editProductModel.Price },
                { "CategoryId", editProductModel.CategoryId },
                { "SupplierId", editProductModel.SupplierId },
                { "BrandId", editProductModel.BrandId },
                { "Status", editProductModel.Status },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        if(!string.IsNullOrEmpty(targetPath))
            await ImageExtensions.MoveFile(editProductModel.ImageUrl, targetPath);
        
        return new BaseResponseModel("Updated product success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        // check is already exist
        var product = await _databaseRepository.GetAsync<Product>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (product == null)
            throw new NotFoundException("The product is not found");

        if (!string.IsNullOrEmpty(product.ImageUrl))
            await product.ImageUrl.DeleteImageAsync();

        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "DELETE" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Deleted product success");
    }

    public async Task<BaseResponseModel> DeleteListAsync(List<string> listProductId,
        CancellationToken cancellationToken = default)
    {

        var duplicateId = listProductId.ToList().HasDuplicated(x => x);
        if (duplicateId)
            throw new BadRequestException("Product is duplicate");

        // xem lại
        var tasks = listProductId.Select(async pId =>
        {
            var id = Guid.Parse(pId);
            var p = await FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (p == null)
                throw new NotFoundException("The product is not found");
            return p.ImageUrl;
        }).ToArray();
        
        var images = await Task.WhenAll(tasks);
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "DELETE_LIST" },
                { "ListId", string.Join(",", listProductId) },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        var tasksDel = images.Select(async p =>
        {
            if(string.IsNullOrEmpty(p))
                return Task.CompletedTask;
            await p.DeleteImageAsync();
            return Task.CompletedTask;
        });

        await Task.WhenAll(tasksDel);
        
        return new BaseResponseModel("Deleted list product success");
    }

    public async Task<Product> FindByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _databaseRepository.GetAsync<Product>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return product;
    }

    public async Task<BaseResponseModel> ChangeIsBestSellingAsync(Guid productId,
        CancellationToken cancellationToken = default)
    {
        var checkAlreadyExistProduct =
            await CheckAlreadyExistAsync(productId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExistProduct)
            throw new NotFoundException("The product is not found");

        var resultChange = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CHANGE_STATUS_IS_BESTSELLING" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultChange)
            throw new InternalServerException("Change status product best selling failed");
        return new BaseResponseModel("Change status product best selling success");
    }

    public async Task<BaseResponseModel> ChangeIsNewAsync(Guid productId,
        CancellationToken cancellationToken = default)
    {
        var checkAlreadyExistProduct =
            await CheckAlreadyExistAsync(productId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExistProduct)
            throw new NotFoundException("The product is not found");

        var resultChange = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CHANGE_STATUS_IS_NEW" },
                { "Id", productId },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultChange)
            throw new InternalServerException("Change status product new failed");
        return new BaseResponseModel("Change status product new success");
    }

    public async Task<BaseResponseModel> ChangeStatusAsync(Guid productId,
        CancellationToken cancellationToken = default)
    {
        var checkAlreadyExistProduct =
            await CheckAlreadyExistAsync(productId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExistProduct)
            throw new NotFoundException("The product is not found");

        var resultChange = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CHANGE_STATUS" },
                { "Id", productId },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultChange)
            throw new InternalServerException("Change status product failed");
        return new BaseResponseModel("Change status product success");
    }


    // CheckDuplicated: if duplicated returns true, else returns false
    public async Task<bool> CheckDuplicatedAsync(EditProductModel editProductModel,
        CancellationToken cancellationToken = default)
    {
        string? slug = null;
        if (string.IsNullOrEmpty(editProductModel.Slug))
            slug = editProductModel.Name.ConvertToSlug();
        else
            slug = editProductModel.Slug.ConvertToSlug();

        var duplicatedProduct = await _databaseRepository.GetAsync<Product>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CHECK_DUPLICATE" },
                { "Name", editProductModel.Name },
                { "Slug", slug }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return duplicatedProduct != null;
    }

    // AlreadyExist: if already exist returns true, else returns false
    public async Task<bool> CheckAlreadyExistAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _databaseRepository.GetAsync<Product>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return product != null;
    }
}
