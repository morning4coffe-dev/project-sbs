namespace Recurrents.Models;

public partial class Item : ObservableObject
{
    public Item(
        string? id = default,
        string name = "",
        BillingDetails? billing = default,
        int tagId = default,
        bool isNotify = true,
        string description = "",
        DateTime creationDate = default,
        List<Status>? status = default)
    {
        Id = id ?? Guid.NewGuid().ToString("N");
        Name = name;
        Billing = billing ?? new(5, DateOnly.FromDateTime(DateTime.Today));
        TagId = tagId;
        IsNotify = isNotify;
        Description = description;
        CreationDate = creationDate == default ? DateTime.Now : creationDate;
        Status = status ?? [];
    }

    public string Id { get; }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private BillingDetails _billing;

    [ObservableProperty]
    private int tagId;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _isNotify;

    [ObservableProperty]
    private DateTime _creationDate;
    public List<Status> Status { get; }
}
