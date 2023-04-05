using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Models.PurchaseOrder;
using eCommerce.Model.Paginations;
using eCommerce.Model.PurchaseOrders;

namespace eCommerce.Service.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<OkResponseModel<PaginationModel<PurchaseOrderModel>>> GetAllAsync(PurchaseOrderFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    
    Task<OkResponseModel<PurchaseOrderModel>> GetAsync(Guid purchaseOrderId, CancellationToken cancellationToken);
    
    Task<OkResponseModel<eCommerce.Model.PurchaseOrders.PurchaseOrderDetailsModel>> GetDetailsAsync(Guid purchaseOrderId,
        CancellationToken cancellationToken);
    
    Task<BaseResponseModel> CreateAsync(EditPurchaseOrderModel editPurchaseOrderModel, CancellationToken cancellationToken);
    
    Task<BaseResponseModel> UpdateAsync(Guid purchaseOrderId, EditPurchaseOrderModel editPurchaseOrderModel, 
        CancellationToken cancellationToken);
    
    Task<BaseResponseModel> DeleteAsync(Guid purchaseOrderId, CancellationToken cancellationToken);
}