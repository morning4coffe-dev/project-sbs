namespace Recurrents.Presentation;

public partial class ItemEditViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigator;
    private readonly IDialogService _dialog;
    private readonly ICurrencyCache _currency;
    #endregion

    [ObservableProperty]
    private ItemViewModel? _selectedItem;

    public ObservableCollection<string> Currencies { get; } = [];
    public ObservableCollection<string> PaymentMethods { get; }

    public ItemEditViewModel(
        ItemViewModel itemViewModel,
        IStringLocalizer localizer,
        IItemService itemService,
        INavigator navigator,
        IDialogService dialog,
        ICurrencyCache currency)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigator = navigator;
        _dialog = dialog;
        _currency = currency;

        if (itemViewModel is { } itemVM)
        {
            SelectedItem = itemVM;
        }
        else
        {
            SelectedItem = new ItemViewModel(new Item());
        }

        PaymentMethods =
        [
            _localizer["CreditCard"],
            _localizer["DebitCard"],
            _localizer["DigitalWallet"],
            _localizer["BankTransfer"],
            _localizer["Cryptocurrency"],
            _localizer["Invoice"],
            _localizer["Cash"]
        ];

        Load();
    }

    public async void Load()
    {
        try
        {
            var currency = await _currency.GetCurrency(CancellationToken.None);
            Currencies.AddRange(currency!.Rates.Keys);
        }
        catch (Exception ex)
        {

        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedItem is not { })
        {
            throw new InvalidOperationException("No item selected!");
        }

        _itemService.AddOrUpdateItem(SelectedItem);
        await _navigator.NavigateBackWithResultAsync(this, data: SelectedItem);
    }

    [RelayCommand]
    private async Task<bool> Close()
    {
        ContentDialogResult result = ContentDialogResult.Primary;

        //if (IsEditing)
        //{
        //    result = await _dialog.ShowAsync(
        //        _localizer["CloseDialogTitle"],
        //        _localizer["CloseDialogDescription"],
        //        _localizer["Ok"]);
        //}

        if (result == ContentDialogResult.Primary)
        {
            //WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, Canceled: true));
        }

        return result == ContentDialogResult.Primary;
    }

    public async Task<bool> RequestClose() 
    {
        //TODO If no changes found, don't ask, close right away

        var result = await _dialog.ShowAsync(
            _localizer["CloseDialogTitle"],
            _localizer["CloseDialogDescription"],
            _localizer["Ok"]);

        if (result == ContentDialogResult.Primary)
        {
            await Save();
        }

        return result == ContentDialogResult.Primary;
    }
}
