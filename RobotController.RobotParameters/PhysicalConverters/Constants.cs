namespace RobotController.RobotModels.PhysicalConverters
{
    public class Constants
    {
        public const double Uref = 5.0;
        public const double DividerParameter = 110000.0 / 10000.0;
        public const double EncoderDt = 0.01; //10ms sampling
        public const double EncoderResolution = 400; //400 PPR
        public const double EncoderGearRatio = 60 / 40;
        public const double WheelDiameter = 0.195; //195mm
    }
}
