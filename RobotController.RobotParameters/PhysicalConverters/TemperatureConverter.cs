using System;

namespace RobotController.RobotModels.PhysicalConverters
{
    public static class TemperatureConverter
    {
        public static double GetPhysical(ushort temp)
        {
            const double R1 = 10000; //rezystor drugi
            const double Rzero = 10000; //R ntc w 25st
            const double Tzero = 298.15;
            const double B = 3950;
            double Rntc = (1023.0 / temp - 1) * R1;
            double temperature = (1 / Tzero) + (1 / B) * Math.Log(Rntc / Rzero);
            return (1 / temperature) - 273.15;
        }

        public static ushort GetBit(double temperature)
        {
            const double R1 = 10000; //rezystor drugi
            const double Rzero = 10000; //R ntc w 25st
            const double Tzero = 298.15;
            const double B = 3950;
            double expa = Math.Exp(B * (1 / (temperature + 273.15) - 1 / Tzero));
            return (ushort)(1023 / (expa * Rzero / R1 + 1));
        }
    }
}