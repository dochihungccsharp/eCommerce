using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Roles;
using Microsoft.Extensions.Logging;
using Serilog;

namespace eCommerce.Service.Roles;

public class RoleService : IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IRoleService _roleService;
    public RoleService(ILogger<RoleService> logger, IRoleService roleService)
    {
        _logger = logger;
        _roleService = roleService;
    }
    public async Task<OkResponseModel<IList<RoleModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OkResponseModel<RoleModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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