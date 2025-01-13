namespace Recurrents.Models;

public class ItemLog(
    string itemId,
    DateOnly paymentDate,
    decimal price,
    string currencyId)
{
    public string ItemId { get; set; } = itemId;
    public DateOnly PaymentDate { get; set; } = paymentDate;
    public decimal Price { get; set; } = price;
    public string CurrencyId { get; set; } = currencyId;
}
