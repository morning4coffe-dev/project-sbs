namespace Recurrents.Models;

public partial class BillingDetails : ObservableObject
{
    public BillingDetails(
        decimal basePrice, DateOnly initialDate,
        string currencyId = "",
        Period periodType = Period.Monthly, short recurEvery = 1,
        string paymentMethod = "")
    {

        BasePrice = basePrice;
        InitialDate = initialDate;
        CurrencyId = currencyId;
        PeriodType = periodType;
        RecurEvery = recurEvery;
        PaymentMethod = paymentMethod;
    }

    [ObservableProperty]
    private decimal _basePrice;

    [ObservableProperty]
    private DateOnly _initialDate;

    [ObservableProperty]
    private string _currencyId;

    [ObservableProperty]
    private Period _periodType;

    [ObservableProperty]
    private short _recurEvery;

    [ObservableProperty]
    private string _paymentMethod;
}
