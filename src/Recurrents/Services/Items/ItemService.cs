namespace Recurrents.Services.Items;

public class ItemService(
    IDataService dataService,
    INotificationService notification,
    IStringLocalizer localization,
    ISettingsService settings) : IItemService
{
    private readonly IDataService _dataService = dataService;
    private readonly INotificationService _notification = notification;
    private readonly IStringLocalizer _localization = localization;
    private readonly ISettingsService _settings = settings;

    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);
    private bool _isInitialized = false;

    private readonly List<ItemViewModel> _items = [];

    public event EventHandler<ItemViewModel>? OnItemChanged;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _initializationSemaphore.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                return;
            }

            var (items, _) = await _dataService.InitializeDatabaseAsync();

            foreach (var item in items)
            {
                var itemVM = new ItemViewModel(item);
                _items.Add(itemVM);

                if (item?.Billing is { } billing && string.IsNullOrEmpty(billing.CurrencyId))
                {
                    billing.CurrencyId = _settings.DefaultCurrency;
                }

                ScheduleBillingNotifications(itemVM);
            }

            _isInitialized = true;
        }
        finally
        {
            _initializationSemaphore.Release();
        }
    }

    public IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null) =>
        selector is null ? _items : _items.Where(selector);

    public void ClearItems()
    {
        _items.Clear();
        _isInitialized = false;
    }

    public void AddOrUpdateItem(ItemViewModel item)
    {
        var index = _items.IndexOf(item);
        if (index >= 0)
        {
            _items[index] = item;
        }
        else
        {
            _items.Add(item);

            if (item?.Item.Billing is { } billing && string.IsNullOrEmpty(billing.CurrencyId))
            {
                billing.CurrencyId = _settings.DefaultCurrency;
            }
        }

        _ = SaveDataAsync(item);
        ScheduleBillingNotifications(item);
    }

    public void ArchiveItem(ItemViewModel item)
    {
        if (item?.Item is not { } i)
        {
            return;
        }

        i.Status.Add(new(item.IsArchived ? State.Active : State.Archived, DateTime.Now));
        _ = SaveDataAsync(item);
        ScheduleBillingNotifications(item);
    }

    public void DeleteItem(ItemViewModel item)
    {
        _items.Remove(item);
        _ = SaveDataAsync(item);
        ScheduleBillingNotifications(item);
    }

    private async Task SaveDataAsync(ItemViewModel item)
    {
        item.Item.ModifiedDate = DateTime.Now;

        var itemsList = _items.Select(itemViewModel => itemViewModel.Item).ToList();

        if (itemsList is { })
        {
            await _dataService.SaveDataAsync(itemsList!);
        }

        RaiseItemChanged(item);
    }

    private void RaiseItemChanged(ItemViewModel item) => OnItemChanged?.Invoke(this, item);

    public void ScheduleBillingNotifications(ItemViewModel itemVM)
    {
        if (itemVM?.Item is not { } item || !item.IsNotify || !_notification.IsEnabledOnDevice())
        {
            return;
        }

        var futurePayments = itemVM.GetFuturePayments(1);
        foreach (var paymentDate in futurePayments)
        {
            if (_notification.IsNotificationScheduledForDate(paymentDate, item.Id))
            {
                continue;
            }

            var notificationId = Guid.NewGuid().ToString();
            var title = string.Format(_localization["NotificationName"], item.Name);
            var description = string.Format(
                _localization["NotificationDescription"],
                itemVM.FormattedPrice,
                item.Name);

            _notification.ScheduleNotification(
                notificationId,
                title,
                description,
                paymentDate.AddDays(-1),
                _settings.NotificationTime);
        }

        //TODO add notification for the end of notifications for the item
    }
}
