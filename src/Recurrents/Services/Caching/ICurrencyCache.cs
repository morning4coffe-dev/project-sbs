namespace Recurrents.Services.Caching;

public interface ICurrencyCache
{
    DateTime LastSync { get; }

    ValueTask<Currency?> GetCurrency(CancellationToken token);
    ValueTask<decimal> ConvertToDefaultCurrency(decimal value, string currency, string defaultCurrency);
}
