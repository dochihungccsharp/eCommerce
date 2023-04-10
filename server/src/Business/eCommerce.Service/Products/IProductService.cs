using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Paginations;
using eCommerce.Model.Products;

namespace eCommerce.Service.Products;

public interface IProductService
{
    Task<OkResponseModel<PaginationModel<ProductModel>>> GetAllAsync(ProductFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<ProductModel>> GetAsync(Guid productId, CancellationToken cancellationToken);
    Task<OkResponseModel<ProductDetailsModel>> GetDetailsAsync(Guid productId, CancellationToken cancellationToken);
    Task<BaseResponseModel> CreateAsync(EditProductModel editProductModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateAsync(Guid productId, EditProductModel editProductModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ChangeIsBestSellingAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ChangeIsNewAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ChangeStatusAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteListAsync(List<string> listProductId, CancellationToken cancellationToken = default);
    Task<Product> FindByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> CheckDuplicatedAsync(EditProductModel editProductModel, CancellationToken cancellationToken = default);
    Task<bool> CheckAlreadyExistAsync(Guid productId, CancellationToken cancellationToken = default);
}