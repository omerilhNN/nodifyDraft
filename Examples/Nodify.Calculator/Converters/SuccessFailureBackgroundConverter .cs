using Avalonia.Media;
using System.Globalization;
using System;

namespace Nodify.Calculator
{
public class SuccessFailureBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess ? Brushes.LightGreen : Brushes.LightCoral;
        }

        return Brushes.Transparent; // Default color for null or unspecified state
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}
