namespace ToNote.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class BoolToResizeModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool resizeable)
            {
                if (resizeable)
                    return ResizeMode.CanResize;
                else
                    return ResizeMode.NoResize;
            }
            else
                throw new ArgumentException("Wrong data type. Expected a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
