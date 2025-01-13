namespace Recurrents.Presentation;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel => (LoginViewModel)DataContext;

    public LoginPage()
    {
        this.InitializeComponent();
    }
}
