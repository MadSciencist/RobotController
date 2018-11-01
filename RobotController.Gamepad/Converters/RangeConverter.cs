namespace RobotController.Gamepad.Converters
{
    public class RangeConverter
    {
        private readonly short _constrain;
        private readonly float _scaleCoeff;

        public RangeConverter(float scaleCoef, short constrain)
        {
            _constrain = constrain;
            _scaleCoeff = scaleCoef;
        }

        public short ScaleThumbstick(short input)
        {
            var scaled = (short)(input / _scaleCoeff);
            return ConstrainBothDirections(scaled);
        }

        private short ConstrainBothDirections(short input)
        {
            if (input > _constrain) input = _constrain;
            if (input < (short)-_constrain) input = (short)-_constrain;

            return input;
        }
    }
}
