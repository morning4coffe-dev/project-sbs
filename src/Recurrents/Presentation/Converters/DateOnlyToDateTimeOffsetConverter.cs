using Microsoft.UI.Xaml.Data;

namespace Recurrents.Presentation.Converters;

public class DateOnlyToDateTimeOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateOnly dateOnly)
        {
            return new DateTimeOffset(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, TimeSpan.Zero);
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object parameter, string language)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            return new DateOnly(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
        }
        return value ?? DateOnly.FromDateTime(DateTime.Today);
    }
}
