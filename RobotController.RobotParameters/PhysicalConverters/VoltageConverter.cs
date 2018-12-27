namespace RobotController.RobotModels.PhysicalConverters
{
    public static class VoltageConverter
    {
        public static double GetPhysical(ushort volt)
        {
            return (volt * (Constants.Uref / Constants.ConverterBits)) * Constants.VoltageDividerParameter;
        }
        public static ushort GetBit(double voltage)
        {
            return (ushort)(voltage / Constants.VoltageDividerParameter * Constants.ConverterBits / Constants.Uref);
        }
    }
}
