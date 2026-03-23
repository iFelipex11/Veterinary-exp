namespace Veterinary.API.Helpers;

public interface IFileStorage
{
    Task<string> SaveFileAsync(byte[] content, string extension, string containerName);
}
