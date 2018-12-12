using System;

namespace RobotController.RobotModels.PhysicalConverters
{
    public static class VelocityConverter
    {
        public static double GetPhysical(short vel)
        {
            return ((double)vel / (Constants.EncoderDt * Constants.EncoderResolution)) * Constants.EncoderGearRatio * Constants.WheelDiameter;
        }

        public static short GetBit(double velocity)
        {
            throw new NotImplementedException();
        }
    }
}
