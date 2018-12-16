namespace RobotController.RobotModels.PhysicalConverters
{
    public static class VoltageConverter
    {
        public static double GetPhysical(ushort volt)
        {
            return (volt * (Constants.Uref / 1023.0)) * Constants.DividerParameter;
        }
        public static ushort GetBit(double voltage)
        {
            return (ushort)(voltage / Constants.DividerParameter * 1023 / Constants.Uref);
        }
    }
}
