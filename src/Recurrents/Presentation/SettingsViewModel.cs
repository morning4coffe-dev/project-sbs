using System.Globalization;
using WS = Windows.System;

namespace Recurrents.Presentation;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ICurrencyCache _currency;
    private readonly ISettingsService _settingsService;
    //private readonly IItemService _itemService;
    //private readonly IUserService _userService;
    //private readonly INavigation _navigation;
    //private readonly IInteropService _interopService;

    public string AppVersion { get; init; }

    public string LastCurrencySync => _currency.LastSync.ToString("F", CultureInfo.CurrentCulture);

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    public bool _isLoggedIn;

    public bool IsNotificationsEnabled { get; }

    private string _selectedCurrency = string.Empty;

    public string SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            if (_selectedCurrency == value)
            {
                return;
            }

            _selectedCurrency = value;
            _settingsService.DefaultCurrency = value;

            OnPropertyChanged();
        }
    }

    private TimeOnly _notificationTime;
    public TimeOnly NotificationTime
    {
        get => _notificationTime;
        set
        {
            if (_notificationTime == value)
            {
                return;
            }

            _notificationTime = value;
            //_settings.NotificationTime = value;

            //_ = _itemService.GetItems().ForEach(item => item.ScheduleBilling());

            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> Currencies { get; } = [];

    public SettingsViewModel(
        IStringLocalizer localizer,
        ICurrencyCache currencyCache,
        ISettingsService settingsService//,
        //INavigation navigation,
        //INotificationService notificationService,
        //IItemService itemService,
        //IInteropService interopService,
        //IUserService userService
    )
    {
        //_userService = userService;
        _currency = currencyCache;
        _settingsService = settingsService;
        //_itemService = itemService;
        //_navigation = navigation;
        //_interopService = interopService;

        //_userService.OnLoggedInChanged += (s, e) =>
        //{
        //    User = e;
        //    IsLoggedIn = e is { };
        //};

        //IsNotificationsEnabled = notificationService.IsEnabledOnDevice();

        Package package = Package.Current;
        PackageId packageId = package.Id;
        PackageVersion version = packageId.Version;
        AppVersion = string.Format("{0}: {1}.{2}.{3}.{4}", localizer["Version"], version.Major, version.Minor, version.Build, version.Revision);

        Load();
    }

    public async void Load()
    {
        //User = await _userService.RetrieveUser();
        //IsLoggedIn = User is { };

        try
        {
            var currency = await _currency.GetCurrency(CancellationToken.None);
            
            Currencies.AddRange(currency?.Rates.Keys);
        }
        catch
        {
        
        }        

        SelectedCurrency = _settingsService.DefaultCurrency;
        //NotificationTime = _settings.NotificationTime;
    }

    [RelayCommand]
    public async Task LaunchNotificationSettings() =>
        await WS.Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));

    [RelayCommand]
    public async Task LaunchLangSettings() =>
        await WS.Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage-adddisplaylanguage"));


    [RelayCommand]
    private void Login() =>
        throw new Exception();// _navigation.Navigate(typeof(LoginPage));

    [RelayCommand]
    private void Logout()
    {
        throw new Exception();//_navigation.Navigate(typeof(LoginPage));

        //_userService.LogoutAsync();
    }

    [RelayCommand]
    private async Task OpenGithub() =>
        await WS.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents"));

    [RelayCommand]
    private async Task OpenRateAndReview() =>
       await InteropService.OpenStoreReviewUrlAsync();

    [RelayCommand]
    private async Task OpenPrivacyPolicy() =>
        await WS.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/blob/ebf622cb65d60c7d353af69824f63d88fa796bde/privacy-policy.md"));

    [RelayCommand]
    private async Task OpenGithubIssues() =>
        await WS.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/issues/new"));
}
