namespace Recurrents.Services.Billing;

class BillingService : IBillingService
{
    public List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 12)
    {
        var nextBillingDate = GetBillingDates(initialDate, periodType, recurEvery).upcomingBillingDate;

        List<DateOnly> paymentList = [];

        for (int i = 0; i < numberOfPayments; i++)
        {
            DateOnly billingDate = nextBillingDate;
            if (billingDate < initialDate)
            {
                billingDate = billingDate.AddMonths(1);
            }

            paymentList.Add(billingDate);
            nextBillingDate = GetNextBillingDate(billingDate, periodType, recurEvery);
        }

        return paymentList;
    }

    public IEnumerable<DateOnly> GetLastPayments(DateOnly initialDate, Period periodType, int recurEvery)
    {
        var today = DateOnly.FromDateTime(DateTime.Now.Date);
        List<DateOnly> paymentList = [];

        while (initialDate <= today)
        {
            paymentList.Add(initialDate);
            initialDate = GetNextBillingDate(initialDate, periodType, recurEvery);
        }

        return paymentList;
    }

    public (DateOnly lastBillingDate, DateOnly upcomingBillingDate) GetBillingDates(DateOnly initialDate, Period periodType, int recurEvery)
    {
        DateOnly lastBillingDate = initialDate;
        DateOnly nextBillingDate = GetNextBillingDate(initialDate, periodType, recurEvery);

        while (nextBillingDate <= DateOnly.FromDateTime(DateTime.Now.Date))
        {
            lastBillingDate = nextBillingDate;
            nextBillingDate = GetNextBillingDate(lastBillingDate, periodType, recurEvery);
        }

        return (lastBillingDate, nextBillingDate);
    }

    public DateOnly GetNextBillingDate(DateOnly initialDate, Period periodType, int recurEvery)
    {
        return PeriodToTimeSpanMap[periodType](initialDate, recurEvery);
    }

    private readonly Dictionary<Period, Func<DateOnly, int, DateOnly>> PeriodToTimeSpanMap = new()
    {
        { Period.Daily, (date, recurEvery) => date.AddDays(recurEvery) },
        { Period.Weekly, (date, recurEvery) => date.AddDays(recurEvery * 7) },
        { Period.Monthly, (date, recurEvery) => date.AddMonths(recurEvery) },
        { Period.Quarterly, (date, recurEvery) => date.AddMonths(recurEvery * 3) },
        { Period.Annually, (date, recurEvery) => date.AddYears(recurEvery) }
    };
}
