using System;

namespace RobotController.Gamepad.Converters
{
    /// <summary>
    /// this class calculates aray of next integer values of function:
    /// y = A + B * exp(c * x)
    /// and stores in in look up table
    /// This class is made static, to hold the lookups between cycles
    /// and save some computation resources
    /// </summary>
    public static class ExponentialCurve
    {
        public static short[] LookupTable { get; private set; }

        private static short _coefficientB;

        private static short _outputRange = 255;
        //private static double _coefficientA;
        //private static double _coefficientC;

        public static short PerformLookup(short value)
        {
            if (LookupTable == null)
            {
                LookupTable = CalculateLookups();
            }

            return LookupTable[value];
        }

        public static void Update(short coefficient)
        {
            if (coefficient < 0)
            {
                throw new ArgumentException("Coefficient must be in 1-100 range");
            }

            _coefficientB = coefficient;

            LookupTable = CalculateLookups();
        }

        private static short[] CalculateLookups()
        {
            var lut = new short[_outputRange+1];

            var coefficientA = -(double)_coefficientB;
            var coefficientC = (Math.Log(((double)_outputRange - coefficientA) / (double)_coefficientB)) / ((double)_outputRange);

            for (var i = 0; i < (_outputRange+1); i++)
            {
                lut[i] = (short)Math.Ceiling((coefficientA + (double)_coefficientB * Math.Exp(coefficientC * i)));

                lut[i] = Helpers.ConstrainNonnegative(lut[i], _outputRange);
            }

            return lut;
        }
    }
}
