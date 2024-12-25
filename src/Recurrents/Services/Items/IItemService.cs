namespace Recurrents.Services.Items;

public interface IItemService
{
    event EventHandler<ItemViewModel> OnItemChanged;
    Task InitializeAsync();
    IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null);
    void ClearItems();
    void AddOrUpdateItem(ItemViewModel item);
    void ArchiveItem(ItemViewModel item);
    void DeleteItem(ItemViewModel item);
}
