namespace Recurrents.Presentation;

public sealed partial class ItemEdit : Page
{
    public ItemEditViewModel ViewModel => (ItemEditViewModel)DataContext;

    public ItemEdit()
    {
        this.InitializeComponent();

        //SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
    }

    //protected async void OnBackRequested(object? sender, BackRequestedEventArgs e)
    //{
    //    var shouldClose = await ViewModel.RequestClose();

    //    if (shouldClose)
    //    {
    //        //e.
    //    }
    //    else
    //    {
    //        e.Handled = true;
    //    }
    //}
}
