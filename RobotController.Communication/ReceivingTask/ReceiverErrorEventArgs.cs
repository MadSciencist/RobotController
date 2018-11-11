using System;
using System.IO;

namespace RobotController.Communication.ReceivingTask
{
    public class ReceiverErrorEventArgs : ErrorEventArgs
    {
        public ReceiverErrorEventArgs(Exception exception) : base(exception)
        {
        }
    }
}
