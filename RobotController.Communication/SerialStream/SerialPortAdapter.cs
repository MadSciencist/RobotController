using System;
using System.Diagnostics;
using System.IO.Ports;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortAdapter : IStreamResource
    {
        private SerialPort _serialPort;

        public SerialPortAdapter(SerialPort serialPort)
        {
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");

            _serialPort = serialPort;
        }

        public int InfiniteTimeout
        {
            get { return SerialPort.InfiniteTimeout; }
        }

        public int ReadTimeout
        {
            get { return _serialPort.ReadTimeout; }
            set { _serialPort.ReadTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return _serialPort.WriteTimeout; }
            set { _serialPort.WriteTimeout = value; }
        }

        public void DiscardInBuffer()
        {
            if (!_serialPort.IsOpen) return;
            _serialPort.DiscardInBuffer();
        }

        public int BytesToRead()
        {
            return _serialPort.IsOpen ? _serialPort.BytesToRead : -1;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.IsOpen ? _serialPort.Read(buffer, offset, count) : -1;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (!_serialPort.IsOpen) return;
            _serialPort.Write(buffer, offset, count);
        }
    }
}
