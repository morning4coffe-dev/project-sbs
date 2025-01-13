using Microsoft.UI.Xaml.Data;

namespace Recurrents.Presentation.Converters;

public class PaymentPeriodToLocalizedStringConverter : IValueConverter
{
    private IStringLocalizer _localizer;

    public PaymentPeriodToLocalizedStringConverter()
    {
        _localizer = App.Services!.GetRequiredService<IStringLocalizer>();
    }

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Period period)
        {
            return _localizer[period.ToString()].Value;
        }

        return value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
