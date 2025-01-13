namespace Recurrents.Presentation;

public partial class MainViewModel : ObservableObject
{
    private readonly IStringLocalizer _localizer;
    private readonly INavigator _navigation;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private bool _indicateLoading;

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigation)
    {
        _localizer = localizer;
        _navigation = navigation;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSecond = new AsyncRelayCommand(GoToSecondView);
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    private async Task GoToSecondView()
    {
        /*await _navigator.NavigateViewModelAsync<ThirdViewModel>(this, data: new Entity(Name!));*/
    }

}
