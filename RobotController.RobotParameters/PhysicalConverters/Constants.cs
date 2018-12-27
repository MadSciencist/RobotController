namespace RobotController.RobotModels.PhysicalConverters
{
    public class Constants
    {
        public const double Uref = 3.3;
        public const double VoltageDividerParameter = 100000.0 / 9100.0;
        public const double ConverterBits = 1023.0;
        public const double EncoderDt = 0.01; //10ms sampling
        public const double EncoderResolution = 400; //400 PPR
        public const double EncoderGearRatio = 60.0 / 40.0;
        public const double WheelDiameter = 0.195; //195mm
    }
}
