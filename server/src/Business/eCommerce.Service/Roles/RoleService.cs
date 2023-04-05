using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.RoleRepository;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Roles;
using eCommerce.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Serilog;
using InvalidOperationException = eCommerce.Shared.Exceptions.InvalidOperationException;

namespace eCommerce.Service.Roles;

public class RoleService : IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly string SQL_QUERY = "sp_Roles";
    public RoleService(
        ILogger<RoleService> logger, 
        IDatabaseRepository databaseRepository,
        IRoleRepository roleRepository,
        IMapper mapper
        )
    {
        _logger = logger;
        _databaseRepository = databaseRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }
    public async Task<OkResponseModel<IList<RoleModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _databaseRepository.GetAllAsync<RoleModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL"}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        return new OkResponseModel<IList<RoleModel>>(roles.ToList());
    }

    public async Task<OkResponseModel<RoleModel>> GetAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _databaseRepository.GetAsync<RoleModel>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "FIND_ROLE_BY_ID"},
                {"Id", roleId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        if (role == null)
            throw new BadRequestException("The request is invalid");
        return new OkResponseModel<RoleModel>(role);
    }

    public async Task<BaseResponseModel> CreateAsync(EditRoleModel editRoleModel, CancellationToken cancellationToken = default)
    {
        var role = _mapper.Map<Role>(editRoleModel);

        var duplicateRole = await _roleRepository.CheckDuplicateRole(role, cancellationToken).ConfigureAwait(false);
        if(!duplicateRole)
            throw new InvalidOperationException("Role with the same name already exists.");

        var resultCreated = await _roleRepository.CreateRoleAsync(role, cancellationToken).ConfigureAwait(false);
        if(!resultCreated)
            throw new InternalServerException("Create role fail");
        
        return new BaseResponseModel("Create role success");
    }

    public async Task<BaseResponseModel> UpdateAsync(Guid roleId, EditRoleModel editRoleModel, CancellationToken cancellationToken = default)
    {
        var r = await _roleRepository.FindRoleByIdAsync(roleId, cancellationToken).ConfigureAwait(false);
        if (r == null)
            throw new BadRequestException("The role id is not found");
        
        var role = _mapper.Map<Role>(editRoleModel);
        role.Id = roleId;

        await _roleRepository.UpdateRoleAsync(role, cancellationToken).ConfigureAwait(false);

        return new BaseResponseModel("Update role success");
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var r = await _roleRepository.FindRoleByIdAsync(roleId, cancellationToken).ConfigureAwait(false);
        if (r != null)
            throw new BadRequestException("The role id is not found");
        
        await _roleRepository.DeleteRoleAsync(roleId, cancellationToken).ConfigureAwait(false);
        
        return new BaseResponseModel("Deleted role success");
        
    }
}