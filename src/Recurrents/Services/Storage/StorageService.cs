using System.Collections.Concurrent;

namespace Recurrents.Services.Storage;

public class StorageService : IStorageService
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    public bool DoesFileExist(string fileName)
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        var filePath = Path.Combine(localFolder.Path, fileName);
        return File.Exists(filePath);
    }

    public async Task<string> ReadFileAsync(string relativeLocalPath, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(relativeLocalPath))
        {
            return string.Empty;
        }

        IStorageItem targetLocation = await ApplicationData.Current.LocalFolder.TryGetItemAsync(relativeLocalPath);

        ct.ThrowIfCancellationRequested();

        if (targetLocation is StorageFile file)
        {
            return await FileIO.ReadTextAsync(file);
        }

        return string.Empty;
    }

    public async Task<bool> WriteFileAsync(string content, string relativeLocalPath)
    {
        if (string.IsNullOrEmpty(relativeLocalPath))
        {
            return false;
        }

        var semaphore = _semaphores.GetOrAdd(relativeLocalPath, new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            StorageFile targetLocation = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                relativeLocalPath,
                CreationCollisionOption.OpenIfExists);

            if (targetLocation != null)
            {
                await FileIO.WriteTextAsync(targetLocation, content);
            }
        }
        catch (Exception ex)
        {
            //TODO: Log exception
            return false;
        }
        finally
        {
            semaphore.Release();
        }

        return true;
    }

    public async Task<bool> DeleteFileAsync(string absolutePathInLocalStorage)
    {
        if (string.IsNullOrEmpty(absolutePathInLocalStorage))
        {
            return false;
        }

        try
        {
            //TODO Add support for deleting Remote files
            var file = await StorageFile.GetFileFromPathAsync(absolutePathInLocalStorage);
            await file.DeleteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
