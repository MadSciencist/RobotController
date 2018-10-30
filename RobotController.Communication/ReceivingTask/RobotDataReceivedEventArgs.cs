using System;

namespace RobotController.Communication.ReceivingTask
{
    public class RobotDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
        public int Length { get; set; }
    }
}
