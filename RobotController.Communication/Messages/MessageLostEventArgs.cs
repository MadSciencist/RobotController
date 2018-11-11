using System;

namespace RobotController.Communication.Messages
{
    public class MessageLostEventArgs : EventArgs
    {
        public int TotalLostCount { get; set; }
    }
}
