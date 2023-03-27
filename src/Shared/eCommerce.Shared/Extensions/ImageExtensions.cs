using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Shared.Extensions;

public static class ImageExtensions
{
    public static async Task<string> SaveImageAsync(this IFormFile file, IWebHostEnvironment _env)
    {
        if (file == null)
            throw new NullReferenceException("Invalid file upload");// file upload không hợp lệ
        
        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
        string fileExtension = Path.GetExtension(file.FileName);
        string newFileName = fileName + "_" + Guid.NewGuid() + fileExtension;
        string filePath  = Path.Combine(_env.WebRootPath, "uploads", newFileName);
        using var fileStream = new FileStream(filePath, FileMode.Create);
        file.CopyToAsync(fileStream);
        return filePath.Trim();

    }

    public static async Task<List<string>> SaveImagesAsync(this IList<IFormFile> files, IWebHostEnvironment _env)
    {
        if (files == null || files.Count < 1)
            return default!;
        
        var listFilePath = new List<String>();
        try
        {
            foreach (var file in files)
            {
                var filePath = await file.SaveImageAsync(_env);
                listFilePath.Add(filePath.Trim());
            }
        }
        catch
        {
            foreach (var filePath in listFilePath)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            throw new IOException("File upload failed"); 
        }
        return listFilePath;
    }
    
    public static async Task<string> MoveFile(IWebHostEnvironment _env, string sourcePath, string? targetPath = null)
    {
        ArgumentNullException.ThrowIfNull(_env);
        
        if (!File.Exists(sourcePath))
            throw new IOException("File does not exist"); 
        
        string fileName = Path.GetFileName(sourcePath);
        string destFile = String.Empty;
        if (targetPath == null)
        {
            destFile = Path.Combine(_env.WebRootPath, "images", fileName);
        }
        else
        {
            destFile = Path.Combine(targetPath, fileName);
        }
        
        File.Move(sourcePath, destFile);
        return destFile;
        
    }

    public static async Task DeleteImageAsync(this string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    public static async Task DeleteImagesAsync(this List<string> listFilePath)
    {
        foreach (var filePath in listFilePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
    
    public static async Task DeleteFilesInDirectory(string path)
    {
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

}
