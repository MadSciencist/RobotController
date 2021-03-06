﻿using System.IO.Ports;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortAdapter : IStreamResource
    {
        private readonly SerialPort _serialPort;

        public SerialPortAdapter(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public int ReadTimeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _serialPort.WriteTimeout;
            set => _serialPort.WriteTimeout = value;
        }

        public void DiscardInBuffer()
        {
            if (!_serialPort.IsOpen) return;
            _serialPort.DiscardInBuffer();
        }

        public int BytesToRead()
        {
            return _serialPort.IsOpen ? _serialPort.BytesToRead : 0;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.IsOpen ? _serialPort.Read(buffer, offset, count) : 0;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (!_serialPort.IsOpen) return;
            _serialPort.Write(buffer, offset, count);
        }
    }
}
