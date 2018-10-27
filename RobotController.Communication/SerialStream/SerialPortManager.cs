using System;
using System.IO.Ports;

namespace RobotController.Communication.SerialStream
{
    public class SerialPortManager
    {
        public EventHandler<ConnectionEventArgs> ConnectionChanged;
        public EventHandler<SerialPortEventArgs> ErrorOccured;

        public void TryOpen(SerialPort serialPort)
        {
            if (serialPort.IsOpen)
            {
                return;
            }

            try
            {
                serialPort.Open();

                ConnectionChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = true, PortName = serialPort.PortName });
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, new SerialPortEventArgs(e));
            }
        }

        public void Close(SerialPort serialPort)
        {
            serialPort.DiscardInBuffer();
            serialPort.Close();
            serialPort.Dispose();

            ConnectionChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = false, PortName = string.Empty });
        }
    }
}
