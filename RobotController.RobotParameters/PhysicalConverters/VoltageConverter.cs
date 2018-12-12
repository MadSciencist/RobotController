namespace RobotController.RobotModels.PhysicalConverters
{
    public static class VoltageConverter
    {
        public static double GetPhysical(short volt)
        {
            return (volt * (Constants.Uref / 1023.0)) * Constants.DividerParameter;
        }
        public static short GetBit(double voltage)
        {
            return (short)(voltage / Constants.DividerParameter * 1023 / Constants.Uref);
        }
    }
}
