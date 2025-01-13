namespace Recurrents.Presentation;

public sealed partial class ArchivePage : Page
{
    public ArchiveViewModel ViewModel => (ArchiveViewModel)DataContext;

    public ArchivePage()
    {
        InitializeComponent();

        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
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
