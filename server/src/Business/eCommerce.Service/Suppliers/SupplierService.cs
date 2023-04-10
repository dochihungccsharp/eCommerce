using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Paginations;
using eCommerce.Model.Suppliers;
using eCommerce.Shared.Exceptions;
using InvalidOperationException = System.InvalidOperationException;

namespace eCommerce.Service.Suppliers;

 public class SupplierService : ISupplierService
    {
        private const string SQL_QUERY = "sp_Suppliers";
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IMapper _mapper;

        public SupplierService(IDatabaseRepository databaseRepository, IMapper mapper)
        {
            _databaseRepository = databaseRepository;
            _mapper = mapper;
        }
        public async Task<OkResponseModel<PaginationModel<SupplierModel>>> GetAllAsync(SupplierFilterRequestModel filter,
            CancellationToken cancellationToken = default)
        {
            var suppliers = await _databaseRepository.PagingAllAsync<Supplier>(
                sqlQuery: SQL_QUERY,
                pageIndex: filter.PageIndex, 
                pageSize: filter.PageSize,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "GET_ALL" },
                    { "SearchString", filter.SearchString }
                },
                cancellationToken: cancellationToken
                ).ConfigureAwait(false);

            return new OkResponseModel<PaginationModel<SupplierModel>>(_mapper.Map<PaginationModel<SupplierModel>>(suppliers));
        }

        public async Task<OkResponseModel<SupplierModel>> GetAsync(Guid supplierId, CancellationToken cancellationToken)
        {
            var supplier = await _databaseRepository.GetAsync<SupplierModel>(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "GET_BY_ID" }, 
                    { "Id", supplierId }
                },
                cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            
            if(supplier == null)
                throw new NotFoundException("The supplier is not found");

            return new OkResponseModel<SupplierModel>(supplier);
        }

        public async Task<OkResponseModel<SupplierDetailsModel>> GetDetailsAsync(Guid supplierId, CancellationToken cancellationToken)
        {
            var supplier = await _databaseRepository.GetAsync<SupplierDetailsModel>(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "GET_DETAILS_BY_ID" }, 
                    { "Id", supplierId }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            if(supplier == null)
                throw new NotFoundException("The supplier is not found");

            return new OkResponseModel<SupplierDetailsModel>(supplier);
        }

        public async Task<BaseResponseModel> CreateAsync(EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default)
        {
            var checkDuplicatedSupplier = await CheckDuplicatedAsync(editSupplierModel, cancellationToken).ConfigureAwait(false);
            if (checkDuplicatedSupplier)
                throw new InvalidOperationException("Supplier with the same name or phone or email already exists");

            await _databaseRepository.ExecuteAsync(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "INSERT" },
                    { "Id", Guid.NewGuid() },
                    { "Name", editSupplierModel.Name },
                    { "Description", editSupplierModel.Description },
                    { "Address", editSupplierModel.Address },
                    { "Phone", editSupplierModel.Phone },
                    { "Email", editSupplierModel.Email },
                    { "ContactPerson", editSupplierModel.ContactPerson },
                    { "Status", editSupplierModel.Status  }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            return new BaseResponseModel("Created supplier success");
        }

        public async Task<BaseResponseModel> UpdateAsync(Guid supplierId, EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default)
        {
            var checkAlreadyExistSupplier = await CheckAlreadyExistAsync(supplierId, cancellationToken).ConfigureAwait(false);
            if(!checkAlreadyExistSupplier)
                throw new NotFoundException("The supplier is not found");

            await _databaseRepository.ExecuteAsync(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "UPDATE" },
                    { "Id", supplierId },
                    { "Name", editSupplierModel.Name },
                    { "Description", editSupplierModel.Description },
                    { "Address", editSupplierModel.Address },
                    { "Phone", editSupplierModel.Phone },
                    { "Email", editSupplierModel.Email },
                    { "ContactPerson", editSupplierModel.ContactPerson },
                    { "Status", editSupplierModel.Status  }
                }, 
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            return new BaseResponseModel("Update supplier success");
        }
        

        public async Task<BaseResponseModel> DeleteAsync(Guid supplierId, CancellationToken cancellationToken = default)
        {
            var checkAlreadyExist = await CheckAlreadyExistAsync(supplierId, cancellationToken).ConfigureAwait(false);
            
            if(!checkAlreadyExist)
                throw new NotFoundException("The supplier is not found");

            await _databaseRepository.ExecuteAsync(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "DELETE" },
                    { "Id", supplierId }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            return new BaseResponseModel("Deleted supplier success");
        }

        public async Task<BaseResponseModel> ChangeStatusAsync(Guid supplierId, CancellationToken cancellationToken)
        {
            var checkAlreadyExist = await CheckAlreadyExistAsync(supplierId, cancellationToken).ConfigureAwait(false);
            
            if(!checkAlreadyExist)
                throw new NotFoundException("The supplier is not found");
            
            await _databaseRepository.ExecuteAsync(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "CHANGE_STATUS" }, 
                    { "Id", supplierId }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            return new BaseResponseModel("Change status supplier success");
        }

        public async Task<bool> CheckDuplicatedAsync(EditSupplierModel editSupplierModel, CancellationToken cancellationToken = default)
        {
            var duplicatedSupplier = await _databaseRepository.GetAsync<Supplier>(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "CHECK_DUPLICATE" },
                    { "Name", editSupplierModel.Name },
                    { "Phone", editSupplierModel.Phone },
                    { "Email", editSupplierModel.Email }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            return duplicatedSupplier != null;
        }

        public async Task<bool> CheckAlreadyExistAsync(Guid supplierId, CancellationToken cancellationToken = default)
        {
            var supplier = await _databaseRepository.GetAsync<Supplier>(
                sqlQuery: SQL_QUERY,
                parameters: new Dictionary<string, object>()
                {
                    { "Activity", "GET_BY_ID" }, 
                    { "Id", supplierId }
                },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            return supplier != null;
        }
    }