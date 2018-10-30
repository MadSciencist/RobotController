using RobotController.Communication.ReceivingTask;
using System;

namespace RobotController.Communication.Interfaces
{
    public interface IReceiverTask
    {
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<ReceiverErrorEventArgs> ErrorOccurred;

        void Start();
        void Stop();
    }
}