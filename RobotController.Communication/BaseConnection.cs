using System;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace RobotController.Communication
{
    public class BaseConnection : IDisposable
    {
        public delegate void ConnectedEventHandler(object o, ConnectionEventArgs args);
        public event ConnectedEventHandler OnConnect;

        public delegate void DisconnectedEventHandler(object o, ConnectionEventArgs args);
        public event DisconnectedEventHandler OnDisconnect;

        public delegate void PortInUseEventHandler(object o, ConnectionEventArgs args);
        public event PortInUseEventHandler PortInUse;

        public delegate void InvalidOperationEventHandler(object o, ConnectionEventArgs args);
        public event InvalidOperationEventHandler InvalidPortOperation;

        public int BaudRate { get; set; } = 115200;
        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public int ReadTimeout { get; set; } = 250;
        public int WriteTimeout { get; set; } = 250;
        public int ReadBufferSize { get; set; } = 2048;
        public int WriteBufferSize { get; set; } = 512;
        public Handshake Handshake { get; set; } = Handshake.None;
        public Encoding Encoding { get; set; } = Encoding.GetEncoding(28591);
        public bool DtrEnable { get; set; } = false;

        private readonly SerialPort _serialPort;

        public BaseConnection()
        {
            _serialPort = new SerialPort
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
                Encoding = Encoding
            };

        }
        public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        protected void TryToOpenPort(string portName)
        {
            _serialPort.PortName = portName;

            try
            {
                _serialPort.Open();
                OnConnect?.Invoke(this, new ConnectionEventArgs { IsConnected = true, PortName = portName });
            }
            catch (UnauthorizedAccessException e)
            {
                PortInUse?.Invoke(this, new ConnectionEventArgs { ExceptionMessage = e.Message });
            }
            catch (InvalidOperationException e)
            {
                InvalidPortOperation?.Invoke(this, new ConnectionEventArgs { ExceptionMessage = e.Message });
            }
            catch (Exception e)
            {
            }
        }

        protected void ClosePort()
        {
            _serialPort.Close();
            OnDisconnect?.Invoke(this, new ConnectionEventArgs { IsConnected = false });
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }
    }
}
