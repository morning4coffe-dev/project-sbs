namespace Recurrents.Presentation;

public partial class ItemViewModel : ObservableObject
{
    private readonly IBillingService _billingService;
    //private readonly INotificationService _notification;
    private readonly IStringLocalizer _localization;
    private readonly ISettingsService _settingsService;
    private readonly ICurrencyCache _currency;

    public ItemViewModel(Item item)
    {
        _item = item;

        _billingService = App.Services?.GetRequiredService<IBillingService>()!;
        //#if !HAS_UNO || __ANDROID__
        //        _notification = App.Services?.GetRequiredService<INotificationService>()!;
        //#endif
        _localization = App.Services?.GetRequiredService<IStringLocalizer>()!;
        _settingsService = App.Services?.GetRequiredService<ISettingsService>()!;
        _currency = App.Services?.GetService<ICurrencyCache>()!;

        if (item?.Billing is { } billing && string.IsNullOrEmpty(billing.CurrencyId))
        {
            billing.CurrencyId = _settingsService.DefaultCurrency;
        }

        ScheduleBilling();
    }

    [ObservableProperty]
    private Item _item;

    public Tag? DisplayTag
    {
        get
        {
            if (Item is not { } item)
            {
                return null;
            }

            return null;//App.Services?.GetRequiredService<ITagService>().Tags.FirstOrDefault(tag => tag.Id == item.TagId);
        }
    }

    public int PaymentDate => (GetFuturePayments(1).First().ToDateTime(new TimeOnly()) - DateTime.Today).Days;

    public string BillingCycle => _localization[Item?.Billing.PeriodType.ToString() ?? "N/A"];
    
    public string? PaymentMethod => 
        string.IsNullOrEmpty(Item?.Billing.PaymentMethod) 
        ? "N/A" 
        : Item?.Billing.PaymentMethod;

    public string FormattedPaymentDate
    {
        get
        {
            var remaining = PaymentDate;

            if (remaining == 1)
            {
                return $"{_localization["Tomorrow"]}".ToLower();
            }

            return $"{_localization["InTime"]} {remaining.ToString() ?? "N/A"} {_localization["Days"]}";
        }
    }

    public decimal TotalPrice
    {
        get
        {
            if (Item?.Billing is not { } billing)
            {
                return 0M;
            }

            var dates = _billingService.GetLastPayments(billing.InitialDate, billing.PeriodType, billing.RecurEvery);

            //TODO Doesn't account for currency or the previous value of the currency
            return Enumerable.Count(dates) * billing.BasePrice;
        }
    }

    public string FormattedTotalPrice =>
        TotalPrice.ToString("C", CurrencyCache.CurrencyCultures[Item?.Billing.CurrencyId ?? "EUR"]);

    public string FormattedPrice =>
        $"{Item?.Billing.BasePrice.ToString("C", CurrencyCache.CurrencyCultures[Item?.Billing.CurrencyId ?? "EUR"])}";
        //var task = _currency.ConvertToDefaultCurrency(
        //    Item?.Billing.BasePrice ?? 0,
        //    Item?.Billing.CurrencyId ?? "EUR",
        //    _settingsService.DefaultCurrency);


    private Status? GetLastStatus()
    {
        if (Item?.Status is not { } itemStatus || Item?.Status.Count == 0)
        {
            return null;
        }

        return itemStatus.OrderByDescending(s => s.Date).First();
    }

    public bool IsArchived => GetLastStatus()?.State == State.Archived;

    public DateOnly? ArchiveDate
    {
        get
        {
            if (GetLastStatus() is not { } dateTime)
            {
                return null;
            }

            return DateOnly.FromDateTime(dateTime.Date);
        }
    }

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
        {
            return [];
        }

        return _billingService.GetFuturePayments(
            billing.InitialDate,
            billing.PeriodType,
            billing.RecurEvery,
            numberOfPayments);
    }

    public IEnumerable<DateOnly> GetLastPayments()
    {
        if (Item?.Billing is not { } billing)
        {
            return [];
        }

        return _billingService.GetLastPayments(
            billing.InitialDate, 
            billing.PeriodType, 
            billing.RecurEvery);
    }

    public void Updated()
    {
        ScheduleBilling();
    }

#pragma warning disable CS1998
    // Async method lacks 'await' operators and will run synchronously as it is currently implemented only for Windows
    public async void ScheduleBilling()
    {
#if !HAS_UNO
        if (Item is not { } item || !Item.IsNotify)
        {
            return;
        }

        //_notification.RemoveScheduledNotifications();

        //if (!_notification.IsEnabledOnDevice())
        //{
        //    //TODO Notify user that notifications are disabled
        //    return;
        //}


        var paymentDates = GetFuturePayments();

        var tasks = paymentDates.Select(date =>
        Task.Run(() =>
        {
            var title = string.Format(_localization["NotificationName"], item.Name);
            //(≈{2})
            //var text = string.Format(_localization["NotificationDescription"], item.Billing.BasePrice.ToString("C", CurrencyCache.CurrencyCultures[Item?.Billing.CurrencyId ?? "EUR"]), item.Name);

            //- time period before e.g. one day, time that user selected
            //DEBUG
            //_notification.ScheduleNotification(item.Id, title, text, DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now.AddSeconds(2)));

            //_notification.ScheduleNotification(item.Id, title, text, date.AddDays(-1), _settingsService.NotificationTime);
        })).ToArray();

        await Task.WhenAll(tasks);
#endif
    }
}
