using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Files;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Service.Uploads;

public interface IUploadService
{
    Task<OkResponseModel<FileModel>> UploadFile(IFormFile file, CancellationToken cancellationToken = default);
    Task<OkResponseModel<IList<FileModel>>> UploadFiles(IList<IFormFile> file, CancellationToken cancellationToken = default);
}