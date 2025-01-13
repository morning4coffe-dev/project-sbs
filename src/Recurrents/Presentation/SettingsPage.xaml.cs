namespace Recurrents.Presentation;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel => (SettingsViewModel)DataContext;

    public SettingsPage()
    {
        this.InitializeComponent();
        this.Loaded += Page_Loaded;
        this.Unloaded += Page_Unloaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        //ViewModel.Unload();
    }
}
