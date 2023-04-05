using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Paginations;
using eCommerce.Model.Suppliers;

namespace eCommerce.Service.Suppliers;

public interface ISupplierService
{
    Task<OkResponseModel<PaginationModel<SupplierModel>>> GetAllAsync(SupplierFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<SupplierModel>> GetAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<OkResponseModel<SupplierDetailsModel>> GetDetailsAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateAsync(EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateAsync(Guid supplierId, EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> ChangeStatusAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<bool> CheckDuplicatedAsync(EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default);
    Task<bool> CheckAlreadyExistAsync(Guid supplierId, CancellationToken cancellationToken = default);
}