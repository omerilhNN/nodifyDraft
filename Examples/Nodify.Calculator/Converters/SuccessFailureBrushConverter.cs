using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Nodify.Calculator
{

public class SuccessFailureBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess
                ? new LinearGradientBrush
                {
                    GradientStops = new GradientStops
                    {
                        new GradientStop { Color = Colors.Green, Offset = 0 },
                        new GradientStop { Color = Colors.DarkGreen, Offset = 1 }
                    }
                }
                : new LinearGradientBrush
                {
                    GradientStops = new GradientStops
                    {
                        new GradientStop { Color = Colors.Red, Offset = 0 },
                        new GradientStop { Color = Colors.DarkRed, Offset = 1 }
                    }
                };
        }

        // Varsayılan renk
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
}

