namespace RobotController.Gamepad.Converters
{
    public static class Helpers
    {
        public static short ConstrainSymetrical(short input, short constrain)
        {
            if (input > constrain) input = constrain;
            if (input < -constrain) input = (short) -constrain;

            return input;
        }
        public static short ConstrainNonnegative(short input, short constrain)
        {
            if (input < 0) input = 0;
            if (input > constrain) input = constrain;

            return input;
        }
    }
}
