using eCommerce.Domain.Domains;
using eCommerce.Model.Abstractions.Responses;

namespace eCommerce.Service.Provinces;

public interface IProvinceService
{
    Task<OkResponseModel<IList<Province>>> GetAllProvinceAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<IList<District>>> GetAllDistrictAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<IList<District>>> GetAllDistrictByProvinceIdAsync(Guid provinceId ,CancellationToken cancellationToken = default);
    Task<OkResponseModel<IList<Ward>>> GetAllWardAsync(CancellationToken cancellationToken = default);
    Task<OkResponseModel<IList<Ward>>> GetAllWardByDistrictIdAsync(Guid districtId, CancellationToken cancellationToken = default);
}