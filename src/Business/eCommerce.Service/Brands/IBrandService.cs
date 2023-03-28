using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Brands;
using eCommerce.Model.Paginations;

namespace eCommerce.Service.Brands;

public interface IBrandService
{
    Task<OkResponseModel<PaginationModel<BrandModel>>> GetAllAsync(BrandFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<BrandModel>> GetAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<OkResponseModel<BrandDetailsModel>> GetDetailsAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateAsync(EditBrandModel editBrandModel, CancellationToken cancellationToken);
    Task<BaseResponseModel> UpdateAsync(Guid brandId, EditBrandModel editBrandModel, CancellationToken cancellationToken);
    Task<BaseResponseModel> ChangeStatusAsync(Guid brandId, CancellationToken cancellationToken);
    Task<BaseResponseModel> DeleteAsync(Guid brandId, CancellationToken cancellationToken);
    Task<Brand> FindByIdAsync(Guid brandId, CancellationToken cancellationToken);
    Task<bool> CheckDuplicatedAsync(EditBrandModel editProductModel, CancellationToken cancellationToken = default);
    Task<bool> CheckAlreadyExistAsync(Guid brandId, CancellationToken cancellationToken = default);
}