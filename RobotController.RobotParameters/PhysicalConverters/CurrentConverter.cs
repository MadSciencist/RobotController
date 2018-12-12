namespace RobotController.RobotModels.PhysicalConverters
{
    public static class CurrentConverter
    {
        public static double GetPhysical(short bitValue)
        {
            const double sensitivity = 66.0; //mV/amp - for ACS712
            // const double sensitivity = 30.25; //for ACS + 1mR shunt in parallel
            const int offset = 2500; // mV
            return (((bitValue * (Constants.Uref * 1000 / 1023.0)) - offset) / sensitivity);
        }

        public static short GetBit(double current)
        {
            const double sensitivity = 66.0; //mV/amp - for ACS712
            //const double sensitivity = 30.25; //for ACS + 1mR shunt in parallel
            const int offset = 2500; // mV
            return (short)(((current * sensitivity + offset) * 1023 / 1000) / Constants.Uref);
        }
    }
}
