namespace Recurrents.Presentation;

public partial class ItemViewModel : ObservableObject
{
    private readonly IBillingService _billing;
    private readonly IStringLocalizer _localization;
    private readonly ISettingsService _settings;
    private readonly ICurrencyCache _currency;

    private const string DEFAULT_CURRENCY = "EUR";

    public ItemViewModel(Item item)
    {
        _item = item ?? throw new ArgumentNullException(nameof(item));
        _billing = App.Services?.GetRequiredService<IBillingService>() ?? throw new ArgumentNullException(nameof(_billing));
        _localization = App.Services?.GetRequiredService<IStringLocalizer>() ?? throw new ArgumentNullException(nameof(_localization));
        _settings = App.Services?.GetRequiredService<ISettingsService>() ?? throw new ArgumentNullException(nameof(_settings));
        _currency = App.Services?.GetRequiredService<ICurrencyCache>() ?? throw new ArgumentNullException(nameof(_currency));
    }

    [ObservableProperty]
    private Item _item;

    public Tag? DisplayTag => new(0, "Test", System.Drawing.Color.Aqua);
    //Item?.TagId != null                                                                            
    //? App.Services?.GetRequiredService<ITagService>().Tags.FirstOrDefault(tag => tag.Id == Item.TagId)                                                              
    //: null;

    public int PaymentDate => (GetFuturePayments(1).FirstOrDefault().ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

    public string BillingCycle => _localization[Item?.Billing?.PeriodType.ToString() ?? "N/A"];

    public string PaymentMethod => Item?.Billing?.PaymentMethod ?? "N/A";

    public string FormattedPaymentDate => PaymentDate == 1
                ? _localization["Tomorrow"].ToString().ToLower()
                : $"{_localization["InTime"]} {PaymentDate} {_localization["Days"]}";

    public decimal TotalPrice
    {
        get
        {
            if (Item?.Billing is not { } billing)
                return 0M;

            var dates = _billing.GetLastPayments(
                billing.InitialDate,
                billing.PeriodType,
                billing.RecurEvery);

            return dates.Count() * billing.BasePrice;
        }
    }

    public string FormattedTotalPrice => FormatPrice(TotalPrice);

    public string FormattedPrice => FormatPrice(Item?.Billing?.BasePrice ?? 0M);

    private string FormatPrice(decimal amount)
    {
        var currencyId = Item?.Billing?.CurrencyId ?? DEFAULT_CURRENCY;
        return amount.ToString("C", CurrencyCache.CurrencyCultures[currencyId]);
    }

    private Status? GetLastStatus() =>
        Item?.Status?.OrderByDescending(s => s.Date).FirstOrDefault();

    public bool IsArchived => GetLastStatus()?.State == State.Archived;

    public DateOnly? ArchiveDate =>
        GetLastStatus() is { } status ? DateOnly.FromDateTime(status.Date) : null;

    public int GetPaymentsInPeriod(int periodDays, int offsetDays = 0)
    {
        var dates = GetLastPayments().ToArray();
        var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-((periodDays + offsetDays) - 1)));
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-(offsetDays - 1)));

        return dates.Count(date => date >= startDate && date <= endDate);
    }

    public IEnumerable<DateOnly> GetFuturePayments(int numberOfPayments = 12)
    {
        if (Item?.Billing is not { } billing)
            return Array.Empty<DateOnly>();

        return _billing.GetFuturePayments(
            billing.InitialDate,
            billing.PeriodType,
            billing.RecurEvery,
            numberOfPayments);
    }

    public IEnumerable<DateOnly> GetLastPayments()
    {
        if (Item?.Billing is not { } billing)
            return Array.Empty<DateOnly>();

        return _billing.GetLastPayments(
            billing.InitialDate,
            billing.PeriodType,
            billing.RecurEvery);
    }
}
