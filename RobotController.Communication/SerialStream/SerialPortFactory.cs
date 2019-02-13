using RobotController.Communication.Interfaces;
using System.IO.Ports;
using System.Text;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortFactory : ISerialPortFactory
    {
        public int BaudRate { get; set; } = 57600;
        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public int ReadTimeout { get; set; } = -1;
        public int WriteTimeout { get; set; } = 250;
        public int ReadBufferSize { get; set; } = 32;
        public int WriteBufferSize { get; set; } = 512;
        public Handshake Handshake { get; set; } = Handshake.None;
        public Encoding Encoding { get; set; } = Encoding.GetEncoding(28591);
        public bool DtrEnable { get; set; } = false;
        public int ReceivedBytesThreshold { get; set; } = 10;


        public SerialPort GetPort(string portName, int baudRate)
        {
            return new SerialPort(portName)
            {
                //setup constant port parameters
                BaudRate = BaudRate,
                DataBits = DataBits,
                Parity = Parity,
                StopBits = StopBits,
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout,
                ReadBufferSize = ReadBufferSize,
                WriteBufferSize = WriteBufferSize,
                Handshake = Handshake,
                DtrEnable = DtrEnable,
                Encoding = Encoding,
                ReceivedBytesThreshold = ReceivedBytesThreshold
            };
        }

        public SerialPort GetPort(string portName)
        {
            return GetPort(portName, BaudRate);
        }
    }
}