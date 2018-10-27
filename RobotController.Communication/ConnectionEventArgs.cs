using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Communication
{
    public class ConnectionEventArgs : EventArgs
    {
        public string PortName { get; set; }
        public bool IsConnected { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
