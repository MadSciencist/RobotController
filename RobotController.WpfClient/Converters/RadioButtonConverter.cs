using System;
using System.Windows.Data;

namespace RobotController.WpfGui.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class RadioButtonConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value) == System.Convert.ToInt32(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return parameter;
        }

        #endregion
    }
}
