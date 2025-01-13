using System.Diagnostics;
using System.Text.Json;

namespace Recurrents.Services.Storage.Data;

public class DataService(
    IStorageService storage/*,
    IUserService userService*/) : IDataService
{
    private readonly IStorageService _storage = storage;
    //private readonly IUserService _userService = userService;

    private const string itemsPath = "recurrents.json";
    private const string logsPath = "recurrents_logs.json";

    public async Task<(List<Item> items, List<ItemLog> logs)> InitializeDatabaseAsync()
    {
        //TODO Check if the remote database is newer than the local one (or vice-versa)

        return (await LoadDataAsync(), null);
    }



    public async Task<bool> SaveDataAsync(List<Item> data)
    {
        var stringData = JsonSerializer.Serialize(data);

        return await SaveAsync(stringData, itemsPath);
    }

    public async Task<bool> SaveLogsAsync(List<ItemLog> logs)
    {
        var stringData = JsonSerializer.Serialize(logs);

        return await SaveAsync(stringData, logsPath);
    }

    private async Task<bool> SaveAsync(string content, string path)
    {
        //if (_userService.IsLoggedIn)
        //{
        //    await _userService.UploadData(content, path);
        //}

        await _storage.WriteFileAsync(content, path);
        //TODO return based on result of both operations
        return true;
    }



    public async Task<List<Item>> LoadDataAsync()
    {
        var data = await LoadAsync(itemsPath, typeof(List<Item>));

        return (data as List<Item>) ?? [];
    }

    public async Task<List<ItemLog>> LoadLogsAsync()
    {
        var logs = await LoadAsync(logsPath, typeof(List<ItemLog>));

        return (logs as List<ItemLog>) ?? [];
    }

    private async Task<object?> LoadAsync(string path, Type type)
    {
        try
        {
            //var isSigned = _userService.IsLoggedIn;
            string remoteContent = "";

            //if (isSigned)
            //{
            //    var data = await _userService.RetrieveData(path, CancellationToken.None);
            //    remoteContent = data.ReadToEnd();
            //}

            //if (!isSigned)
            {
                if (!_storage.DoesFileExist(path))
                {
                    return null;
                }
                var fileContent = await _storage.ReadFileAsync(path);
                remoteContent = fileContent;
            }

            if (remoteContent is not { })
            {
                return null;
            }

            return JsonSerializer.Deserialize(remoteContent, type);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }
}
