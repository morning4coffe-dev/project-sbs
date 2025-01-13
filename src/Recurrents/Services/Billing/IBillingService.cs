namespace Recurrents.Services.Billing;

public interface IBillingService
{
    List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20);
    IEnumerable<DateOnly> GetLastPayments(DateOnly initialDate, Period periodType, int recurEvery);

    (DateOnly lastBillingDate, DateOnly upcomingBillingDate) GetBillingDates(DateOnly date, Period periodType, int recurEvery);
    DateOnly GetNextBillingDate(DateOnly date, Period periodType, int recurEvery);
}
