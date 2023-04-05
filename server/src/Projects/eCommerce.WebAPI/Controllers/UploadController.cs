using eCommerce.Service.Uploads;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.WebAPI.Controllers;

public class UploadController : BaseController
{
    private readonly IUploadService _uploadService;
    public UploadController(ILogger<UploadController> logger, IUploadService uploadService) : base(logger)
    {
        _uploadService = uploadService;
    }
    
    [HttpPost]
    [Route("api/files/upload")]
    public async Task<IActionResult> UploadFile([FromForm]IFormFile file,
        CancellationToken cancellationToken = default)
        => Ok(await _uploadService.UploadFile(file, cancellationToken).ConfigureAwait(false));
    
    [HttpPost]
    [Route("api/files/uploads")]
    public async Task<IActionResult> UploadFiles([FromForm]IList<IFormFile> files,
        CancellationToken cancellationToken = default)
        => Ok(await _uploadService.UploadFiles(files, cancellationToken).ConfigureAwait(false));
}