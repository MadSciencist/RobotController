namespace RobotController.DataLogger
{
    public class DatalogModel
    {
        public short LeftSetpoint { get; set; }
        public short RightSetpoint { get; set; }
        public short LeftSpeed { get; set; }
        public short RightSpeed { get; set; }
        public double LeftCurrent { get; set; }
        public double RightCurrent { get; set; }
        public short Voltage { get; set; }
        public short Temperature { get; set; }
    }
}
