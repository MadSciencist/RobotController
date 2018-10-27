using System.IO.Ports;
using System.Text;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortFactory : ISerialPortFactory
    {
        public int BaudRate { get; set; } = 115200;
        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public int ReadTimeout { get; set; } = -1;
        public int WriteTimeout { get; set; } = 250;
        public int ReadBufferSize { get; set; } = 2048;
        public int WriteBufferSize { get; set; } = 512;
        public Handshake Handshake { get; set; } = Handshake.None;
        public Encoding Encoding { get; set; } = Encoding.GetEncoding(28591);
        public bool DtrEnable { get; set; } = false;
        public int ReceivedBytesThreshold { get; set; } = 10;

        private SerialPort _serialPort;
        private readonly string _portName;

        public SerialPortFactory(string portName) => _portName = portName;

        public SerialPort GetPort()
        {
            _serialPort = new SerialPort(_portName)
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

            return _serialPort;
        }
    }
}