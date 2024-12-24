using System.Reflection;

namespace Recurrents.Presentation;

public partial class HomeViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigation;
    private readonly ICurrencyCache _currency;
    private readonly IDispatcher _dispatcher;
    private readonly ISettingsService _settings;
    #endregion

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    private string _bannerHeader;

    [ObservableProperty]
    private string _sum = "0";

    [ObservableProperty]
    private bool _isItemOpen;

    private ItemViewModel? _selectedItem;
    public ItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem == value)
            {
                return;
            }

            _selectedItem = value;
            IsItemOpen = value is { };
            OnPropertyChanged();

            if (value is { } item)
            {
                _navigation.NavigateViewModelAsync<ItemDetailsViewModel>(this, data: item);
            }
        }
    }

    public ObservableCollection<ItemViewModel> Items { get; } = [];

    public HomeViewModel(
        IItemService itemService,
        IStringLocalizer localizer,
        INavigator navigation,
        ICurrencyCache currency,
        IDispatcher dispatcher,
        ISettingsService settings)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigation = navigation;
        _currency = currency;
        _dispatcher = dispatcher;
        _settings = settings;

        BannerHeader = string.Format(localizer["LastDays"], DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        Sum = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        var informationalVersion = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        WelcomeMessage = DateTime.Now.Hour switch
        {
            >= 5 and < 12 => localizer["GoodMorning"],
            >= 12 and < 17 => localizer["GoodAfternoon"],
            >= 17 and < 20 => localizer["GoodEvening"],
            _ => localizer["GoodNight"]
        };

        Load();
    }

    private List<ItemViewModel> GetItems
        => [.. _itemService
            .GetItems(item => !item.IsArchived)
            .OrderBy(i => i.PaymentDate)];

    [RelayCommand]
    private void AddItem()
    {
        IsItemOpen = true;
    }

    public async void Load()
    {
        if (Items.Count != 0)
        {
            return;
        }

        try
        {
            var currency = await _currency.GetCurrency(CancellationToken.None);
        }
        catch (Exception ex)
        {

        }

        await _itemService.InitializeAsync();

        if (Items.Count > 0)
        {
            Items.Clear();
        }

        if (GetItems is { } items && items.Count > 0)
        {
            SortItems(items);
        }

        RefreshSum(Items);

        _itemService.OnItemChanged += OnItemChanged;
    }

    public void Unload()
    {
        _itemService.OnItemChanged -= OnItemChanged;
    }

    private async void OnItemChanged(object? sender, (ItemViewModel itemVM, Models.Action action) args)
    {
        if (args.itemVM is not { } item)
        {
            return;
        }

        var modifiedItems = Items.ToList();

        switch (args.action)
        {
            case Models.Action.Create:
                modifiedItems.Add(item);
                break;

            case Models.Action.Update:
                var existingItem = modifiedItems.FirstOrDefault(item => item.Item.Id == item.Item.Id);
                if (existingItem != null)
                {
                    var index = modifiedItems.IndexOf(existingItem);
                    modifiedItems[index] = args.itemVM;
                }
                break;

            case Models.Action.Archive:
            case Models.Action.Delete:
                modifiedItems.RemoveAll(item => item.Item.Id == item.Item.Id);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(args.action), "Unexpected action");
        }

        Items.Clear();

        SortItems(modifiedItems);

        SelectedItem = null;

        RefreshSum(Items);
    }

    private async void SortItems(List<ItemViewModel> items)
    {
        var sortedItems = items.OrderBy(i => i.PaymentDate).ThenBy(i => i.Item.Name).ToList();

        await _dispatcher.ExecuteAsync(() =>
        {
            foreach (var item in sortedItems)
            {
                Items.Add(item);
            }
        });
    }

    private async void RefreshSum(IEnumerable<ItemViewModel> items)
    {
        //try
        //{
        //    var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        //    var tasks = items.Select(async item => await _currency.ConvertToDefaultCurrency(
        //        item.Item?.Billing.BasePrice * item.GetPaymentsInPeriod(days) ?? 0,
        //        item?.Item?.Billing?.CurrencyId ?? _settings.DefaultCurrency,
        //        _settings.DefaultCurrency));

        //    var values = await Task.WhenAll(tasks);
        //    var sum = values.Sum();

        //    await _dispatcher.ExecuteAsync(() =>
        //    {
        //        Sum = $"≈ {Math.Round(sum, 2).ToString("C", CurrencyCache.CurrencyCultures[_settings.DefaultCurrency])}";
        //    });
        //}
        //catch
        //{
        //    //TODO: Make show Error more user friendly
        //    Sum = "Connection error.";
        //}
    }
}
