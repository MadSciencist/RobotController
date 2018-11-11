using System;
using System.IO;

namespace RobotController.Communication.SendingTask
{
    public class SenderErrorEventArgs : ErrorEventArgs
    {
        public SenderErrorEventArgs(Exception exception) : base(exception)
        {
        }
    }
}
