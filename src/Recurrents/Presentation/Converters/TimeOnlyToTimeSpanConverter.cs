using Microsoft.UI.Xaml.Data;

namespace Recurrents.Presentation.Converters;

public class TimeOnlyToTimeSpanConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeOnly timeOnly)
        {
            // Assuming you want to create a TimeSpan with the same hours, minutes, and seconds
            return new TimeSpan(timeOnly.Hour, timeOnly.Minute, timeOnly.Second);
        }

        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            // Assuming you want to create a TimeOnly with the same hours, minutes, and seconds
            return new TimeOnly(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        return null;
    }
}
