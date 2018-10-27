using System;

namespace RobotController.Communication.ReceivingTask
{
    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
        public int Length { get; set; }
    }
}
