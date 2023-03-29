using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Orders;
using eCommerce.Model.Paginations;

namespace eCommerce.Service.Orders;

public interface IOrderService
{
    Task<OkResponseModel<PaginationModel<OrderModel>>> GetAllOrder(OrderFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<PaginationModel<OrderModel>>> GetAllOrderByUserId(OrderFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<OkResponseModel<OderDetailsModel>> GetOrderDetailsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> OrderAsync(EditOrderModel editOrderModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CancelOrderAsync(EditOrderModel editOrderModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateOrderAsync(EditOrderModel editOrderModel,
        CancellationToken cancellationToken = default);
}