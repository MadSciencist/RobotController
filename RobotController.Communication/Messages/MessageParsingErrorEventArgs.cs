using System;
using System.IO;

namespace RobotController.Communication.Messages
{
    public class MessageParsingErrorEventArgs : ErrorEventArgs
    {
        public MessageParsingErrorEventArgs(Exception exception) : base(exception) { }
    }
}
