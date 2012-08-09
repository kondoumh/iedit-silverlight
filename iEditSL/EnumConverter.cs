using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace iEditSL
{
    public class EnumConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valueString = value == null ? "????" : value.ToString();

            if (targetType == typeof(string))
                return valueString;

            return string.Compare(valueString, (string)parameter, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(string))
                return Enum.Parse(targetType, (string)value, true);

            if (value.GetType() == typeof(bool))
                return Enum.Parse(targetType, (string)parameter, true);

            if ((Boolean)value)
                return Enum.Parse(targetType, (string)parameter, true);

            return null;
        }
    }
}
