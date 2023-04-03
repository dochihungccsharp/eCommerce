using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Orders;
using eCommerce.Model.Paginations;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;

namespace eCommerce.Service.Orders;

public class OrderService : IOrderService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IMapper _mapper;
    private const string SQL_QUERY = "sp_Orders";
    
    public OrderService(
        IDatabaseRepository databaseRepository,
        IMapper mapper)
    {
        _databaseRepository = databaseRepository;
        _mapper = mapper;
    }
    
    public async Task<OkResponseModel<PaginationModel<OrderModel>>> GetAllOrder(OrderFilterRequestModel filter, CancellationToken cancellationToken = default)
    {
        var orders = await _databaseRepository.PagingAllAsync<Order>(
            sqlQuery: SQL_QUERY,
            pageIndex: filter.PageIndex,
            pageSize: filter.PageSize,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_ALL" },
                { "SearchString", filter.SearchString },
                { "FromTime", filter.FromTime },
                { "ToTime", filter.ToTime },
                { "FromPrice", filter.FromPrice },
                { "ToPrice", filter.ToPrice }, 
                { "IsCancelled", filter.IsCancelled}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<PaginationModel<OrderModel>>(
            _mapper.Map<PaginationModel<OrderModel>>(orders));
    }

    public async Task<BaseResponseModel> UpdateOrderAsync(Guid orderId, UpdateOrderModel updateOrderModel, CancellationToken cancellationToken = default)
    {
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "UPDATE" },
                { "Id", orderId },
                { "OrderStatus", updateOrderModel.OrderStatus },
                { "Note", updateOrderModel.Note }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Update order success");
    }

    public async Task<OkResponseModel<IEnumerable<OrderModel>>> GetAllOrderByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _databaseRepository.GetAllAsync<OrderModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_ALL" },
                { "UserId",  userId},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IEnumerable<OrderModel>>(orders);
    }

    public async Task<OkResponseModel<OrderDetailsModel>> GetOrderDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _databaseRepository.GetAsync<OrderDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_BY_ID" },
                { "Id",  orderId},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<OrderDetailsModel>(order);
    }

    public async Task<BaseResponseModel> OrderAsync(CreateOrderModel createOrderModel, CancellationToken cancellationToken = default)
    {

        if (createOrderModel.OrderItems == null || createOrderModel.OrderItems.Count < 1)
            throw new BadRequestException("Order items is not found");
        else
        {
            var duplicate = createOrderModel.OrderItems.HasDuplicated(x => x.ProductId);
            if(duplicate)  throw new BadRequestException("Order items is duplicate");
        }
        
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "INSERT" },
                {"Id", Guid.NewGuid()},
                { "UserId", createOrderModel.UserId},
                { "PaymentId", createOrderModel.PaymentId},
                { "PromotionId", createOrderModel.PromotionId},
                { "Total", createOrderModel.Total},
                { "PaymentStatus", createOrderModel.PaymentStatus.Trim().ToUpper()},
                { "PaymentMethod", createOrderModel.PaymentMethod.Trim().ToUpper()},
                { "Note", createOrderModel.Note},
                { "OrderItems", createOrderModel.OrderItems.ToDataTable()},
                
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Create order success");
    }

    public async Task<BaseResponseModel> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "CANCEL_ORDER" },
                { "Id",  orderId},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new BaseResponseModel("Cancel order success");
    }
}