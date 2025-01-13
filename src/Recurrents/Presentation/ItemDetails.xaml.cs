
namespace Recurrents.Presentation;

public sealed partial class ItemDetails : Page
{
    public ItemDetailsViewModel ViewModel => (ItemDetailsViewModel)DataContext;

    public ItemDetails()
    {
        this.InitializeComponent();
    }
}
