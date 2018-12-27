namespace RobotController.RobotModels.PhysicalConverters
{
    public static class CurrentConverter
    {
        private const int Offset = 2500; // mV
        private static readonly double Divider = 1800.0 / 2800.0;
        private const double Sensitivity = 66.0; //mV/amp - for ACS712
        // const double sensitivity = 30.25; //for ACS + 1mR shunt in parallel

        public static double GetPhysical(short bitValue)
        {
            bitValue = (short)(bitValue / Divider);
            
            return (bitValue * (Constants.Uref * 1000.0 / Constants.ConverterBits) - Offset) / Sensitivity;
        }

        public static short GetBit(double current)
        {
            var converted = (current * Sensitivity + Offset) * Constants.ConverterBits / 1000.0 / Constants.Uref;
            return (short)(converted * Divider);
        }
    }
}
