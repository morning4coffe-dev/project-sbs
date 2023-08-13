﻿namespace ProjectSBS.Services.FileManagement.Data;

public interface IDataService
{
    Task<(List<Item> items, List<ItemLog> logs)> InitializeDatabaseAsync();
    Task<List<Item>> LoadDataAsync();
    Task<List<ItemLog>> LoadLogsAsync();

    Task<bool> SaveDataAsync(List<Item> data);
    Task<bool> SaveLogsAsync(List<ItemLog> logs);

    Task<bool> AddLogAsync(ItemLog log);
}