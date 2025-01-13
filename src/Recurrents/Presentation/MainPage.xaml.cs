namespace Recurrents.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        this.InitializeComponent();
    }

#if WINDOWS
    private void TitleBar_PaneToggleRequested(WinUIEx.TitleBar sender, object args)
    {
        navigation.IsPaneOpen = !navigation.IsPaneOpen;
    }
#endif
}
