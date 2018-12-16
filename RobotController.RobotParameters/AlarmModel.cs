using RobotController.RobotModels.PhysicalConverters;

namespace RobotController.RobotModels
{
    public class AlarmModel
    {
        public double VoltageAlarm { get; set; }
        public double CriticalVoltageAlarm { get; set; }
        public double TemperatureAlarm { get; set; }
        public double CriticalTemperatureAlarm { get; set; }
        public double LeftCurrentAlarm { get; set; }
        public double RightCurrentAlarm { get; set; }
    }
}
