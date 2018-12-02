namespace RobotController.Communication.Configuration
{
    public static class ReceiverFraming
    {
        public static char FrameStart { get { return '<'; } }
        public static char FrameEnd { get { return '>'; } }
        public static byte AddressPosition { get { return 1; } }
        public static byte CommandPosition { get { return 2; } }
        public static int FrameLength { get { return 14; } }
        public static int PayloadLength { get { return 8; } }
        public static byte CrcStartByte { get { return 11; } }
        public static byte NumOfBytesToCrcCalculation { get { return 8; } }
    }
}
