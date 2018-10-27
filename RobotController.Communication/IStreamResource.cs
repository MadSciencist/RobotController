using System;

namespace RobotController.Communication
{
    public interface IStreamResource : IDisposable
    {
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        void DiscardInBuffer();

        int BytesToRead();
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
    }
}
