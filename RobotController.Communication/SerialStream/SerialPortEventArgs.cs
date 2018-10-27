using System;
using System.IO;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortEventArgs : ErrorEventArgs
    {
        public SerialPortEventArgs(Exception exception) : base(exception)
        {
        }
    }
}
