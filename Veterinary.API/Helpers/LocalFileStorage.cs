namespace Veterinary.API.Helpers;

public class LocalFileStorage(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : IFileStorage
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<string> SaveFileAsync(byte[] content, string extension, string containerName)
    {
        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var folder = Path.Combine(webRootPath, containerName);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var safeExtension = string.IsNullOrWhiteSpace(extension) ? ".bin" : extension;
        var fileName = $"{Guid.NewGuid()}{safeExtension}";
        var fullPath = Path.Combine(folder, fileName);
        await File.WriteAllBytesAsync(fullPath, content);

        var request = _httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}/{containerName}/{fileName}";
    }
}
