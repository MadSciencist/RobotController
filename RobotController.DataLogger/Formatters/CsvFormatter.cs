using System.Globalization;
using System.Text;

namespace RobotController.DataLogger.Formatters
{
    public class CsvFormatter : IFormatter
    {
        public string Format(DatalogModel log)
        {
            var sb = new StringBuilder();

            sb.Append(log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture))
               .Append(',')
              .Append(log.LeftSetpoint)
              .Append(',')
              .Append(log.RightSetpoint)
              .Append(',')
              .Append(log.LeftSpeed)
              .Append(',')
              .Append(log.RightSpeed)
              .Append(',')
              .Append(log.RawLeftSpeed)
              .Append(',')
              .Append(log.RawRightSpeed)
              .Append(',')
              .Append(log.LeftCurrent)
              .Append(',')
              .Append(log.RawLeftCurrent)
              .Append(',')
              .Append(log.RightCurrent)
              .Append(',')
              .Append(log.RawRightCurrent)
              .Append(',')
              .Append(log.Voltage)
              .Append(',')
              .Append(log.Temperature);

            return sb.ToString();
        }

        public string GetHeader()
        {
            return "TIMESTAMP, LEFT SETPOINT, RIGHT SETPOINT, LEFT VEL, RIGHT VEL, LEFT RAW VEL, RIGHT RAW VEL, LEFT CURR, LEFT RAW CURR, RIGHT CURR, RIGHT RAW CURR, VOLT, TEMP";
        }
    }
}
