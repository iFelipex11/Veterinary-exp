namespace Veterinary.API.Helpers;

public class LocalFileStorage(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : IFileStorage
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<string> SaveFileAsync(byte[] content, string extension, string containerName)
    {
        var folder = Path.Combine(_environment.WebRootPath, containerName);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(folder, fileName);
        await File.WriteAllBytesAsync(fullPath, content);

        var request = _httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}/{containerName}/{fileName}";
    }
}
