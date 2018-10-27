using System;

namespace RobotController.Communication.ReceivingTask
{
    public interface IReceiverTask
    {
        EReceiverStatus Status { get; }

        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<ReceiverErrorEventArgs> ErrorOccurred;

        void Cancel();
        void Close();
        void Dispose();
        void Start();
        void Stop();
    }
}