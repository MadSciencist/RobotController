using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Communication
{
    public interface IConnection
    {
        void ConnectToPort(string portName);
        void Disconnect();
        string[] GetAvailablePorts();
    }
}
