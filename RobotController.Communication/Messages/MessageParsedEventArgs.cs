using RobotController.Communication.Interfaces;
using System;

namespace RobotController.Communication.Messages
{
    public class MessageParsedEventArgs : EventArgs
    {
        public IMessage Message { get; set; }
    }
}
