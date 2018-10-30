namespace RobotController.Communication.Configuration
{
    public static class Framing
    {
        public static char FrameStart { get { return '<'; } }
        public static char FrameEnd { get { return '>'; } }
        public static int FrameLength { get { return 16; } }
        public static int PayloadLength { get { return 10; } }
        public static byte CrcStartByte { get { return 13; } }
        public static byte NumOfBytesToCrcCalculation { get { return 12; } }
    }
}
