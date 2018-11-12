using System;
using System.IO.Ports;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortManager
    {
        public EventHandler<ConnectionEventArgs> ConnectionChanged;
        public EventHandler<SerialPortEventArgs> ErrorOccured;

        private readonly SerialPort _serialPort;

        public SerialPortManager(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public void TryOpen()
        {
            if (_serialPort.IsOpen) return;

            try
            {
                _serialPort.Open();

                DiscardInBuffer();
                DiscardOutBuffer();

                ConnectionChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = true, PortName = _serialPort.PortName });
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, new SerialPortEventArgs(e));
            }
        }

        public void Close()
        {
            if (!_serialPort.IsOpen) return;

            DiscardInBuffer();
            DiscardOutBuffer();
            _serialPort.Close();

            ConnectionChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = false, PortName = string.Empty });
        }

        public void DiscardInBuffer()
        {
            if(_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
            }
        }

        public void DiscardOutBuffer()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DiscardOutBuffer();
            }
        }
    }
}
