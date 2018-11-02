using System;

namespace RobotController.Gamepad.Converters
{
    /// <summary>
    /// this class calculates aray of next integer values of function:
    /// y = A + B*exp(c*x)
    /// and stores in in look up table
    /// This class is made static, to hold the lookups between cycles
    /// and save some computation resources
    /// </summary>
    public static class ExponentialCurve
    {
        private static short _coefficientB;
        private static double _coefficientA;
        private static double _coefficientC;
        private static short[] _lookupTable;

        public static short PerformLookup(short value, short coefficient)
        {
            //if the coefficient has changed, recalculate lookups
            if (coefficient == _coefficientB)
            {
                return _lookupTable[value];
            }

            if (coefficient < 0)
            {
                throw new ArgumentException("Coefficient must be in 1-100 range");
            }

            _coefficientB = coefficient;
            _lookupTable = CalculateLookups();
            return _lookupTable[value];

        }

        private static short[] CalculateLookups()
        {
            var lut = new short[256];

            _coefficientA = -(double)_coefficientB;
            _coefficientC = (Math.Log((255.0 - _coefficientA) / (double)_coefficientB)) / 255.0;

            for (var i = 0; i < 256; i++)
            {
                lut[i] = (short)Math.Ceiling((_coefficientA + (double)_coefficientB * Math.Exp(_coefficientC * i)));

                Helpres.ConstrainNonnegative(lut[i], 255);
            }

            return lut;
        }
    }
}
