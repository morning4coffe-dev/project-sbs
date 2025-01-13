namespace Recurrents.Services.Storage;

public interface IStorageService
{
    bool DoesFileExist(string fileName);

    Task<string> ReadFileAsync(string relativeLocalPath, CancellationToken ct = default);
    Task<bool> WriteFileAsync(string content, string relativeLocalPath);
    Task<bool> DeleteFileAsync(string absolutePathInLocalStorage);
}