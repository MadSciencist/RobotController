namespace RobotController.DataLogger
{
    public class DatalogModel
    {
        public short LeftSetpoint { get; set; }
        public short RightSetpoint { get; set; }
        public double LeftSpeed { get; set; }
        public short RawLeftSpeed { get; set; }
        public double RightSpeed { get; set; }
        public short RawRightSpeed { get; set; }
        public double LeftCurrent { get; set; }
        public short RawLeftCurrent { get; set; }
        public double RightCurrent { get; set; }
        public short RawRightCurrent { get; set; }
        public double Voltage { get; set; }
        public double Temperature { get; set; }
    }
}
