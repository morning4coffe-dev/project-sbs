using System.Globalization;

namespace Recurrents.Services.Caching;

public sealed class CurrencyCache(
    IApiClient api,
    ISerializer serializer,
    ILogger<CurrencyCache> logger) : ICurrencyCache
{
    private readonly IApiClient _api = api;
    private readonly ISerializer _serializer = serializer;
    private readonly ILogger _logger = logger;

    private static readonly SemaphoreSlim fileLock = new(1, 1);

    private Currency? _current;
    public DateTime LastSync { get; private set; }

    public static IReadOnlyDictionary<string, CultureInfo> CurrencyCultures = new Dictionary<string, CultureInfo>
    {
        { "AUD", new CultureInfo("en-AU") },
        { "BGN", new CultureInfo("bg-BG") },
        { "BRL", new CultureInfo("pt-BR") },
        { "CAD", new CultureInfo("en-CA") },
        { "CHF", new CultureInfo("fr-CH") },
        { "CNY", new CultureInfo("zh-CN") },
        { "CZK", new CultureInfo("cs-CZ") },
        { "DKK", new CultureInfo("da-DK") },
        { "GBP", new CultureInfo("en-GB") },
        { "HKD", new CultureInfo("zh-HK") },
        { "HUF", new CultureInfo("hu-HU") },
        { "IDR", new CultureInfo("id-ID") },
        { "ILS", new CultureInfo("he-IL") },
        { "INR", new CultureInfo("en-IN") },
        { "ISK", new CultureInfo("is-IS") },
        { "JPY", new CultureInfo("ja-JP") },
        { "KRW", new CultureInfo("ko-KR") },
        { "MXN", new CultureInfo("es-MX") },
        { "MYR", new CultureInfo("ms-MY") },
        { "NOK", new CultureInfo("nb-NO") },
        { "NZD", new CultureInfo("en-NZ") },
        { "PHP", new CultureInfo("en-PH") },
        { "PLN", new CultureInfo("pl-PL") },
        { "RON", new CultureInfo("ro-RO") },
        { "SEK", new CultureInfo("sv-SE") },
        { "SGD", new CultureInfo("en-SG") },
        { "THB", new CultureInfo("th-TH") },
        { "TRY", new CultureInfo("tr-TR") },
        { "USD", new CultureInfo("en-US") },
        { "ZAR", new CultureInfo("en-ZA") },
        { "EUR", new CultureInfo("en-EU") }
    };

    private static bool IsConnected
    {
        get
        {
            var networkProfile = NetworkInformation.GetInternetConnectionProfile();

            if (networkProfile is not null)
            {
                return networkProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }

            return false;
        }
    }

    public async ValueTask<Currency?> GetCurrency(CancellationToken token)
    {
        if (_current?.Rates is { })
        {
            return _current;
        }

        var currencyText = await GetCachedCurrency();
        if (!string.IsNullOrWhiteSpace(currencyText))
        {
            return _serializer.FromString<Currency>(currencyText);
        }

        if (!IsConnected)
        {
            _logger.LogWarning("App is offline and cannot connect to the API.");
            throw new Exception("No internet connection");
        }

        var response = await _api.GetCurrency(token);

        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            _current = (Currency?)response.Content;
            _current.Rates.TryAdd("EUR", 1);

            _ = Task.Run(() => Save(_current, token));
            return _current;
        }
        else if (response.Error is not null)
        {
            _logger.LogError(response.Error, "An error occurred while retrieving the latest Currencies.");
            throw response.Error;
        }
        else
        {
            return null;
        }
    }

    public async ValueTask<decimal> ConvertToDefaultCurrency(decimal value, string currency, string defaultCurrency = "EUR")
    {
        _current ??= await GetCurrency(CancellationToken.None) ?? new();

        decimal conversionRateToDefault = Convert.ToDecimal(_current.Rates[currency]);
        decimal conversionRateFromDefault = Convert.ToDecimal(_current.Rates[defaultCurrency]);

        return (value / conversionRateToDefault) * conversionRateFromDefault;
    }

    private static async ValueTask<StorageFile?> GetFile(CreationCollisionOption option)
    {
        await fileLock.WaitAsync();

        try
        {
            return await ApplicationData.Current.TemporaryFolder.CreateFileAsync("currency.json", option);
        }
        catch
        {
            return null;
        }
        finally
        {
            fileLock.Release();
        }
    }

    private async ValueTask<string?> GetCachedCurrency()
    {
        var file = await GetFile(CreationCollisionOption.OpenIfExists);

        if (file is null)
        {
            return null;
        }

        var properties = await file.GetBasicPropertiesAsync();

        // Request data if offline
        // or the file is younger than 3 days
        if (!IsConnected || (IsConnected && DateTimeOffset.Now.AddDays(-3) <= properties.DateModified))
        {
            try
            {
                LastSync = properties.DateModified.DateTime;
                return await File.ReadAllTextAsync(file.Path);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    private async ValueTask Save(Currency currency, CancellationToken token)
    {
        var currencyText = _serializer.ToString(currency);
        var file = await GetFile(CreationCollisionOption.ReplaceExisting);

        if (file is null)
        {
            return;
        }
        await File.WriteAllTextAsync(file.Path, currencyText, token);
    }
}
