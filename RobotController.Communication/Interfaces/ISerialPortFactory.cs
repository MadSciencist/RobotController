using System.IO.Ports;

namespace RobotController.Communication.Interfaces
{
    public interface ISerialPortFactory
    {
        SerialPort GetPort(string portName, int baudRate);
        SerialPort GetPort(string portName);
    }
}