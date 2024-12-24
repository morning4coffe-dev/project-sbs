namespace Recurrents.Presentation;

public partial class ArchiveViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigation;
    private readonly IDialogService _dialog;
    private readonly IDispatcher _dispatcher;
    #endregion

    [ObservableProperty]
    public ItemViewModel? _selectedItem;

    public ObservableCollection<ItemViewModel> Items { get; } = [];

    public ArchiveViewModel(
        IItemService itemService,
        IStringLocalizer localizer,
        INavigator navigation,
        IDialogService dialog,
        IDispatcher dispatcher)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigation = navigation;
        _dialog = dialog;
        _dispatcher = dispatcher;

        RefreshItems();
    }

    private void RefreshItems()
    {
        var effectiveItems = _itemService.GetItems(item => item.IsArchived)
                                         .OrderBy(i => i.PaymentDate)
                                         .ToList();
        _dispatcher.TryEnqueue(() =>
        {
            Items.Clear();
            Items.AddRange(effectiveItems);
        });
    }


    [RelayCommand]
    public async Task Archive(ItemViewModel? item = null)
    {
        if (item is not { } && SelectedItem is not { })
        {
            return;
        }

        _itemService.ArchiveItem(item ?? SelectedItem);

        //AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Archived",
        //    (item ?? SelectedItem).IsArchived.ToString());

        SelectedItem = null;
        RefreshItems();
    }

    [RelayCommand]
    public async Task Delete(ItemViewModel? item = null)
    {
        if (item is not { } && SelectedItem is not { })
        {
            return;
        }

        var result = await _dialog.ShowAsync(
            _localizer["DeleteDialogTitle"],
            _localizer["DeleteDialogDescription"],
            _localizer["Delete"]);

        if (result == ContentDialogResult.Primary)
        {
            _itemService.DeleteItem(item ?? SelectedItem);

            //AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Deleted", "True");

            SelectedItem = null;
            RefreshItems();
        }
    }

    [RelayCommand]
    private void OpenSettings()
        => _navigation.NavigateViewModelAsync<SettingsViewModel>(this);
}
