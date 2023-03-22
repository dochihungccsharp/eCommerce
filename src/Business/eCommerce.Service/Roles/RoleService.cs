using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Roles;
using eCommerce.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Serilog;

namespace eCommerce.Service.Roles;

public class RoleService : IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IDatabaseRepository _databaseRepository;
    public RoleService(ILogger<RoleService> logger, IDatabaseRepository databaseRepository)
    {
        _logger = logger;
        _databaseRepository = databaseRepository;
    }
    public async Task<OkResponseModel<IList<RoleModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _databaseRepository.GetAllAsync<RoleModel>(
            sqlQuery: "sp_GetAllRole",
            parameters: null,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return new OkResponseModel<IList<RoleModel>>(roles.ToList());
    }

    public async Task<OkResponseModel<RoleModel>> GetAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _databaseRepository.GetAsync<RoleModel>(
            "sp_GetRole",
            parameters: new Dictionary<string, object>()
            {

            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (role == null)
            throw new BadRequestException("The request is invalid");
        return new OkResponseModel<RoleModel>(role);
    }

    public async Task<BaseResponseModel> CreateAsync(EditRoleModel editRoleModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid roleId, EditRoleModel editRoleModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}