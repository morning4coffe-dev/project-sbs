namespace Recurrents.Models;

public partial class Item : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private BillingDetails _billing = new(5, DateOnly.FromDateTime(DateTime.Today));

    [ObservableProperty]
    private int tagId;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _isNotify = true;

    [ObservableProperty]
    private DateTime _creationDate = DateTime.Now;

    public List<Status> Status { get; set; } = new();

    [ObservableProperty]
    private DateTime _modifiedDate = DateTime.Now;
}
