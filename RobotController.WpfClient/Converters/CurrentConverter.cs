using System;
using System.Globalization;
using System.Windows.Data;

namespace RobotController.WpfGui.Converters
{
    public class CurrentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RobotController.RobotModels.PhysicalConverters.CurrentConverter.GetBit(System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
