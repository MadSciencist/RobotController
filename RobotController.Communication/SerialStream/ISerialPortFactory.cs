using System.IO.Ports;

namespace RobotController.Communication.SerialStream
{
    public interface ISerialPortFactory
    {
        SerialPort GetPort();
    }
}