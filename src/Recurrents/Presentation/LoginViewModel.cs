using Windows.System;

namespace Recurrents.Presentation;

public partial class LoginViewModel : ObservableObject
{
    private readonly INavigator _navigation;
    //private readonly IUserService _userService;

    //private readonly INavigation _navigation;
    //private readonly IItemService _itemService;

    [ObservableProperty]
    private bool _indicateLoading;

    public LoginViewModel(
        INavigator navigation
        //INavigation navigation,
        //IUserService userService,
        //IItemService itemService
        )
    {
        _navigation = navigation;
        //_userService = userService;
        //_itemService = itemService;

        //itemService.ClearItems();
    }

    [RelayCommand]
    private async Task Login()
    {
        //var success = false;
        //IndicateLoading = true;

        //try
        //{
        //    success = await _userService.AuthenticateAsync();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex);
        //}

        //if (success)
        //{
        //    await _userService.RetrieveUser();
        //    await _itemService.InitializeAsync();

        //    _navigation.Navigate(typeof(MainPage));

        //    App.Services!.GetRequiredService<ISettingsService>().ContinueWithoutLogin = false;
        //    SendAnalytics(true);
        //}

        //IndicateLoading = false;
    }

    private void SendAnalytics(bool loggedIn)
    {
        //Dictionary<string, string> logInStats = new()
        //{
        //    { "Logged In", loggedIn.ToString() },
        //    { "Provider", loggedIn ? "Microsoft" : "N/A" },
        //};

        //AnalyticsService.TrackEvent(AnalyticsService.LogIn, logInStats);
    }

    [RelayCommand]
    private async Task ContinueWithoutLogin()
    {
        IndicateLoading = true;
        //var isLoggedIn = await _userService.AuthenticateAsync(true);
        //if (isLoggedIn)
        //{
        //    await _userService.LogoutAsync();
        //}

        await _navigation.NavigateViewModelAsync<MainViewModel>(this/*, data: new User(Name!)*/);
        
        //App.Services!.GetRequiredService<ISettingsService>().ContinueWithoutLogin = true;
        //_navigation.Navigate(typeof(MainPage));
        IndicateLoading = false;
    }

    [RelayCommand]
    private async Task OpenPrivacyPolicy() =>
        await Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/blob/ebf622cb65d60c7d353af69824f63d88fa796bde/privacy-policy.md"));
}
