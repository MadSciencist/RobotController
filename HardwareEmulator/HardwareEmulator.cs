using System;
using System.IO.Ports;
using HardwareEmulator;

namespace Tests.HardwareEmulator
{
    /// <summary>
    /// Pretty simple class for hardware emulation, so we can test offline
    /// Proper & more reliable communication is implemented in RobotController.Communication
    /// </summary>
    public class HardwareEmulator : IDisposable
    {
        public event EventHandler<string> Received;

        private readonly SerialPort _port;
        private readonly SimpleParser _parser;
        public HardwareEmulator(string port)
        {
            _port = new SerialPort(port, baudRate: 115200, parity: Parity.None, dataBits: 8, stopBits: StopBits.One);
            _port.ReceivedBytesThreshold = 14;
            _port.DataReceived += PortOnDataReceived;

            try
            {
                if (!_port.IsOpen) _port.Open();
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
            }
            catch { }

            _parser = new SimpleParser();
            _parser.Received += (sender, s) => Received?.Invoke(sender, s);
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = new byte[14];
            _port.Read(data, 0, _port.BytesToRead);

            _parser.Parse(data);
        }

        public void Write(byte[] data)
        {
            _port.Write(data, 0, data.Length);
        }


        public void Dispose()
        {
            if (_port.IsOpen) _port.Close();
            _port?.Dispose();
        }
    }
}
