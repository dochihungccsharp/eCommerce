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
                { "FromTime", filter.FromTime },
                { "ToTime", filter.ToTime },
                { "FromPrice", filter.FromPrice },
                { "ToPrice", filter.ToPrice },
                { "IsBestSelling", filter.IsBestSelling },
                { "IsNew", filter.IsNew },
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

        // check duplicate
        var checkDuplicatedProduct =
            await CheckDuplicatedAsync(editProductModel, cancellationToken).ConfigureAwait(false);
        if (checkDuplicatedProduct)
            throw new InvalidOperationException("Product with the same name already exists.");

        // handler image product
        string? imageUrl = null;
        if (editProductModel.ImageUpload != null)
            imageUrl = await editProductModel.ImageUpload.SaveImageAsync(_env);
        
        string? slug = null;
        if (string.IsNullOrEmpty(editProductModel.Slug))
            slug = editProductModel.Name.ConvertToSlug();
        else
            slug = editProductModel.Slug.ConvertToSlug();


        // create product
        var resultCreated = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "INSERT" },
                { "Id", Guid.NewGuid() },
                { "Name", editProductModel.Name },
                { "Slug", slug },
                { "Description", editProductModel.Description },
                { "ImageUrl", imageUrl },
                { "OriginalPrice", editProductModel.OriginalPrice },
                { "Price", editProductModel.Price },
                { "CategoryId", editProductModel.CategoryId },
                { "SupplierId", editProductModel.SupplierId },
                { "BrandId", editProductModel.BrandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultCreated)
            throw new InternalServerException("Created product failed");

        return new BaseResponseModel("Create product success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid productId, EditProductModel editProductModel,
        CancellationToken cancellationToken = default)
    {
        // check is already exist
        var checkAlreadyExistProduct =
            await CheckAlreadyExistAsync(productId, cancellationToken).ConfigureAwait(false);
        if (!checkAlreadyExistProduct)
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
        string? imageUrl = null;
        if (editProductModel.ImageUpload != null)
            imageUrl = await editProductModel.ImageUpload.SaveImageAsync(_env);

        string? slug = null;
        if (string.IsNullOrEmpty(editProductModel.Slug))
            slug = editProductModel.Name.ConvertToSlug();
        else
            slug = editProductModel.Slug.ConvertToSlug();

        // update
        var resultUpdated = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE" },
                { "Id", productId },
                { "Name", editProductModel.Name },
                { "Slug", slug },
                { "Description", editProductModel.Description },
                { "ImageUrl", imageUrl },
                { "OriginalPrice", editProductModel.OriginalPrice },
                { "Price", editProductModel.Price },
                { "CategoryId", editProductModel.CategoryId },
                { "SupplierId", editProductModel.SupplierId },
                { "BrandId", editProductModel.BrandId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultUpdated)
            throw new InternalServerException("Updated product failed");
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

        var resultDeleted = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "DELETE" },
                { "Id", productId }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultDeleted)
            throw new InternalServerException("Deleted product failed");
        return new BaseResponseModel("Deleted product success");
    }

    public async Task<BaseResponseModel> DeleteListAsync(string[] listProductId,
        CancellationToken cancellationToken = default)
    {
        if (!listProductId.ValidateInputIsOfTypeGuid())
            throw new BadRequestException(
                "Product id list is not valid, there may be an element that is not of type guid");

        var resultDeleted = await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "DELETE_LIST" },
                { "ListId", string.Join(",", listProductId) },
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!resultDeleted)
            throw new BadRequestException("Deleted list product failed");
        return new BaseResponseModel("Deleted list product success");
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
