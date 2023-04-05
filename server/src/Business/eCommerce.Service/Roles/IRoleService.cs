using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Roles;

namespace eCommerce.Service.Roles;

public interface IRoleService
{
    Task<OkResponseModel<IList<RoleModel>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<RoleModel>> GetAsync(Guid roleId,CancellationToken cancellationToken = default);
    Task<BaseResponseModel> CreateAsync(EditRoleModel editRoleModel, CancellationToken cancellationToken = default);
    Task<BaseResponseModel> UpdateAsync(Guid roleId, EditRoleModel editRoleModel,
        CancellationToken cancellationToken = default);
    Task<BaseResponseModel> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

}