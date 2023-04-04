using System;
using System.Globalization;
using System.Windows.Data;

namespace FunctionAproximation.WPF;

public class IndexToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index && int.TryParse(parameter.ToString(), out var targetIndex))
        {
            return index == targetIndex;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
