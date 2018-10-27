namespace RobotController.Communication.Configuration
{
    public static class Framing
    {
        public static char FrameStart { get { return '<'; } }
        public static char FrameEnd { get { return '>'; } }
        public static int FrameLength { get { return 14; } }
        public static int PayloadLength { get { return 8; } }

        public static int ReceivingTaskSleepTime { get; internal set; }
    }
}
