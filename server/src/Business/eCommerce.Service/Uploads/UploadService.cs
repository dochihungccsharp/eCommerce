using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Files;
using eCommerce.Shared.Exceptions;
using eCommerce.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Service.Uploads;

public class UploadService : IUploadService
{
    private readonly IWebHostEnvironment _env;

    public UploadService(IWebHostEnvironment env)
    {
        _env = env;
    }
    public async Task<OkResponseModel<FileModel>> UploadFile(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length < 0)
            throw new BadRequestException("File upload invalid");

        var path = await file.SaveImageAsync(_env);

        return new OkResponseModel<FileModel>(new FileModel()
        {
            FilePath = path
        });

    }

    public async Task<OkResponseModel<IList<FileModel>>> UploadFiles(IList<IFormFile> files, CancellationToken cancellationToken = default)
    {
        if (files == null || files.Count < 0)
            throw new BadRequestException("File upload invalid");

        var paths = await files.SaveImagesAsync(_env);
        var fileModels = paths.Select(f => new FileModel() { FilePath = f });
        return new OkResponseModel<IList<FileModel>>(fileModels.ToList());
    }
}