
namespace Recurrents.Presentation;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel => (HomeViewModel)DataContext;

    public HomePage()
    {
        InitializeComponent();
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ViewModel is { })
        {
            ViewModel.SelectedItem = null;

            //ViewModel.RefreshItems();
        }
    }
    }
}
