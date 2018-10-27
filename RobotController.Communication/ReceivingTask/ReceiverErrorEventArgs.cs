using System;
using System.IO;

namespace RobotController.Communication.ReceivingTask
{
    public class ReceiverErrorEventArgs : ErrorEventArgs
    {
        public int NumberOfRestoreAttemps { get; set; }
        public ReceiverErrorEventArgs(Exception exception) : base(exception)
        {
        }
    }
}
