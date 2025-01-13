namespace Recurrents.Services.Settings;

public interface ISettingsService
{
    /// <summary>
    /// Sets a value in the storage.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to identify the value.</param>
    /// <param name="value">The value to be stored.</param>
    void SetValue<T>(string key, T value);

    /// <summary>
    /// Gets a value from the storage.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to identify the value.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The retrieved value or the default value if the key is not found.</returns>
    T GetValue<T>(string key, T defaultValue = default);

    string DefaultCurrency { set; get; }
    TimeOnly NotificationTime { set; get; }
    bool ContinueWithoutLogin { set; get; }
}
