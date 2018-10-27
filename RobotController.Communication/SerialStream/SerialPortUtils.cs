using System.IO.Ports;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortUtils
    {
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
