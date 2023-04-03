using AutoMapper;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Models.PurchaseOrder;
using eCommerce.Model.Paginations;
using eCommerce.Model.PurchaseOrders;
using eCommerce.Service.Suppliers;
using eCommerce.Service.Users;
using eCommerce.Shared.Consts;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;

namespace eCommerce.Service.PurchaseOrders;

public class PurchaseOrderService : IPurchaseOrderService
{
    private const string SQL_QUERY = "sp_PurchaseOrders";
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISupplierService _supplierService;
    private readonly IMapper _mapper;
    public PurchaseOrderService(
        IDatabaseRepository databaseRepository,
        IUserRepository userRepository,
        ISupplierService supplierService,
        IMapper mapper
    )
    {
        _databaseRepository = databaseRepository;
        _userRepository = userRepository;
        _supplierService = supplierService;
        _mapper = mapper;
    }
    
    public async Task<OkResponseModel<PaginationModel<PurchaseOrderModel>>> GetAllAsync(PurchaseOrderFilterRequestModel filter,
        CancellationToken cancellationToken = default)
    {
        var purchaseOrders = await _databaseRepository.PagingAllAsync<Domain.Domains.PurchaseOrder>(
            sqlQuery: SQL_QUERY,
            pageIndex: filter.PageIndex,
            pageSize: filter.PageSize,
            parameters: new Dictionary<string, object>()
            {
                { "Activity", "GET_ALL" },
                { "SearchString", filter.SearchString },
                { "SupplierId", filter.SupplierId },
                { "UserId", filter.UserId },
                { "OrderStatus", filter.PurchaseOrderStatus },
                { "FromPrice", filter.FromPrice },
                { "ToPrice", filter.ToPrice },
                { "FromTime", filter.FromTime },
                { "ToTime", filter.ToTime }
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new OkResponseModel<PaginationModel<PurchaseOrderModel>>(_mapper.Map<PaginationModel<PurchaseOrderModel>>(purchaseOrders));
    }

    public async Task<OkResponseModel<PurchaseOrderModel>> GetAsync(Guid purchaseOrderId, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _databaseRepository.GetAsync<PurchaseOrderModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_BY_ID"},
                {"Id", purchaseOrderId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        if(purchaseOrder == null)
            throw new NotFoundException("The purchase order is not found");

        return new OkResponseModel<PurchaseOrderModel>(purchaseOrder);
    }

    public async Task<OkResponseModel<PurchaseOrderDetailsModel>> GetDetailsAsync(Guid purchaseOrderId, CancellationToken cancellationToken)
    {
        var purchaseOrderDetails = await _databaseRepository.GetAsync<PurchaseOrderDetailsModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_DETAILS_BY_ID"},
                {"Id", purchaseOrderId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        if(purchaseOrderDetails == null)
            throw new NotFoundException("The purchase order is not found");

        return new OkResponseModel<PurchaseOrderDetailsModel>(purchaseOrderDetails);
    }

    public async Task<BaseResponseModel> CreateAsync(EditPurchaseOrderModel editPurchaseOrderModel,
        CancellationToken cancellationToken)
    {
        if (editPurchaseOrderModel.EditPurchaseOrderDetailsModels == null ||
            editPurchaseOrderModel.EditPurchaseOrderDetailsModels?.Count < 1)
            throw new BadRequestException("Your order has no products?");

        var duplicate = editPurchaseOrderModel.EditPurchaseOrderDetailsModels.HasDuplicated(x => x.ProductId);
        if (duplicate)
            throw new NotFoundException("purchase order item is duplicate");
        
        // check is already exist supplier
        var checkAlreadyExistSupplier = await _supplierService
            .CheckAlreadyExistAsync(editPurchaseOrderModel.SupplierId, cancellationToken).ConfigureAwait(false);
        if(!checkAlreadyExistSupplier)
            throw new NotFoundException("The supplier is not found");
        
        // check is already exist user
        var u = await _userRepository
            .FindUserByIdAsync(editPurchaseOrderModel.UserId, cancellationToken).ConfigureAwait(false);
        if(u == null)
            throw new NotFoundException("The user is not found");

        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "INSERT"},
                {"Id", Guid.NewGuid()},
                {"SupplierId", editPurchaseOrderModel.SupplierId},
                {"UserId", editPurchaseOrderModel.UserId},
                {"TotalMoney", editPurchaseOrderModel.TotalMoney},
                {"Note", editPurchaseOrderModel.Note},
                {"OrderStatus", editPurchaseOrderModel.OrderStatus},
                {"PaymentStatus", editPurchaseOrderModel.PaymentStatus},
                {"TotalPaymentAmount", editPurchaseOrderModel.TotalMoney},
                {"PurchaseOrderDetails", editPurchaseOrderModel.EditPurchaseOrderDetailsModels?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Create purchase order success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid purchaseOrderId, EditPurchaseOrderModel editPurchaseOrderModel,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = await _databaseRepository.GetAsync<Domain.Domains.PurchaseOrder>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_BY_ID"},
                {"Id", purchaseOrderId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        if(purchaseOrder == null)
            throw new NotFoundException("The purchase order is not found");

        if (purchaseOrder.OrderStatus == PurchaseOrderStatus.PurchaseInvoice)
            throw new ForbiddenException("Can't edit an imported order");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "UPDATE"},
                {"Id", purchaseOrderId},
                {"SupplierId", editPurchaseOrderModel.SupplierId},
                {"UserId", editPurchaseOrderModel.UserId},
                {"TotalMoney", editPurchaseOrderModel.TotalMoney},
                {"Note", editPurchaseOrderModel.Note},
                {"OrderStatus", editPurchaseOrderModel.OrderStatus},
                {"PaymentStatus", editPurchaseOrderModel.PaymentStatus},
                {"TotalPaymentAmount", editPurchaseOrderModel.TotalMoney},
                {"PurchaseOrderDetails", editPurchaseOrderModel.EditPurchaseOrderDetailsModels?.ToDataTable()}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new BaseResponseModel("Update purchase order success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid purchaseOrderId, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _databaseRepository.GetAsync<Domain.Domains.PurchaseOrder>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_BY_ID"},
                {"Id", purchaseOrderId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        
        if(purchaseOrder == null)
            throw new NotFoundException("The purchase order is not found");

        if (purchaseOrder.OrderStatus == PurchaseOrderStatus.PurchaseInvoice)
            throw new ForbiddenException("Can't delete an imported order");
        
        await _databaseRepository.ExecuteAsync(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "DELETE"},
                {"Id", purchaseOrderId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
       
        return new BaseResponseModel("Deleted purchase order success");
    }
}