using eCommerce.Domain.Domains;
using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Model.Abstractions.Responses;

namespace eCommerce.Service.Provinces;

public class ProvinceService : IProvinceService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly string SQL_QUERY = "sp_Provinces";
    public ProvinceService(IDatabaseRepository databaseRepository)
    {
        _databaseRepository = databaseRepository;
    }
    public async Task<OkResponseModel<IList<Province>>> GetAllProvinceAsync(CancellationToken cancellationToken = default)
    {
        var provinces = await _databaseRepository.GetAllAsync<Province>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL_P"}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IList<Province>>(provinces.ToList());
    }

    public async Task<OkResponseModel<IList<District>>> GetAllDistrictAsync(CancellationToken cancellationToken = default)
    {
        var district = await _databaseRepository.GetAllAsync<District>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL_D"}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IList<District>>(district.ToList());
    }

    public async Task<OkResponseModel<IList<District>>> GetAllDistrictByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        var district = await _databaseRepository.GetAllAsync<District>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL_D_BY_P_ID"},
                {"ProvinceId", provinceId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        return new OkResponseModel<IList<District>>(district.ToList());
    }

    public async Task<OkResponseModel<IList<Ward>>> GetAllWardAsync(CancellationToken cancellationToken = default)
    {
        var wards = await _databaseRepository.GetAllAsync<Ward>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL_W"},
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new OkResponseModel<IList<Ward>>(wards.ToList());
    }

    public async Task<OkResponseModel<IList<Ward>>> GetAllWardByDistrictIdAsync(Guid districtId, CancellationToken cancellationToken = default)
    {
        var wards = await _databaseRepository.GetAllAsync<Ward>(
            sqlQuery: SQL_QUERY,
            parameters: new Dictionary<string, object>()
            {
                {"Activity", "GET_ALL_W_BY_D_ID"},
                {"DistrictId", districtId}
            },
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
        
        return new OkResponseModel<IList<Ward>>(wards.ToList());
    }
}