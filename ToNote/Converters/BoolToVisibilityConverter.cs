using System;
using System.Windows;
using System.Windows.Data;

namespace ToNote.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool visibility)
                return visibility ? Visibility.Visible : Visibility.Collapsed;
            else
                throw new ArgumentException("Wrong data type. Expected a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
