using System;
using System.Globalization;
using System.Windows.Data;

namespace RobotController.WpfGui.Converters
{
    public class VoltageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("convert called");
            return RobotController.RobotModels.PhysicalConverters.VoltageConverter.GetBit(System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
