using System;

namespace RobotController.Communication
{
    public class ConnectionEventArgs : EventArgs
    {
        public string PortName { get; set; }
        public bool IsConnected { get; set; }
    }
}
