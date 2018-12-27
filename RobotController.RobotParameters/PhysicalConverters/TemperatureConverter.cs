using System;

namespace RobotController.RobotModels.PhysicalConverters
{
    public static class TemperatureConverter
    {
        private const double Rzero = 9900.0; //NTC resistance in 25deg C
        private const double R1 = 10000.0; //second resistor resistance
        private const double B = 3950; // NTC beta parameter
        private const double Tzero = 298.15;
        private const double ZeroKelvins = 273.15;

        public static double GetPhysical(ushort temp)
        {
            //TODO voltage conversion
            temp = (ushort) (temp / 5.0 * 3.3);
            var resistanceNtc = (Constants.ConverterBits / temp - 1) * R1;
            var temperature = (1 / Tzero) + (1 / B) * Math.Log(resistanceNtc / Rzero);
            return (1 / temperature) - ZeroKelvins;
        }

        public static ushort GetBit(double temperature)
        {
            var expa = Math.Exp(B * (1 / (temperature + ZeroKelvins) - 1 / Tzero));
            var converted = (ushort) (Constants.ConverterBits / (expa * Rzero / R1 + 1));
            return (ushort) (converted * (5.0 / 3.3));
        }
    }
}