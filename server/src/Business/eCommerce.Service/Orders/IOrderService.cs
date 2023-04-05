using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Orders;
using eCommerce.Model.Paginations;

namespace eCommerce.Service.Orders;

public interface IOrderService
{

    #region Admin
    Task<OkResponseModel<PaginationModel<OrderModel>>> GetAllOrder(OrderFilterRequestModel filter,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateOrderAsync(Guid orderId, UpdateOrderModel updateOrderModel,
        CancellationToken cancellationToken = default);
    #endregion

    #region Customer And Admin
    Task<OkResponseModel<IEnumerable<OrderModel>>> GetAllOrderByUserId(Guid orderId,
        CancellationToken cancellationToken = default);
    
    Task<OkResponseModel<OrderDetailsModel>> GetOrderDetailsAsync(Guid orderId, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Customer
    Task<BaseResponseModel> OrderAsync(CreateOrderModel createOrderModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    #endregion
    
}