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
            var lut = new short[256];

            var coefficientA = -(double)_coefficientB;
            var coefficientC = (Math.Log((255.0 - coefficientA) / (double)_coefficientB)) / 255.0;

            for (var i = 0; i < 256; i++)
            {
                lut[i] = (short)Math.Ceiling((coefficientA + (double)_coefficientB * Math.Exp(coefficientC * i)));

                lut[i] = Helpres.ConstrainNonnegative(lut[i], 255);
            }

            return lut;
        }
    }
}
