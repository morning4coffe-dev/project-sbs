using Microsoft.UI.Xaml.Data;
using System.Drawing;

namespace Recurrents.Presentation.Converters;

public class ColorToBrushConverter : IValueConverter
{

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Color color)
        {
            return new SolidColorBrush(Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color;
        }

        return null;
    }
}
