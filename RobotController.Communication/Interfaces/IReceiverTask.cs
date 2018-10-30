using RobotController.Communication.ReceivingTask;
using System;

namespace RobotController.Communication.Interfaces
{
    public interface IReceiverTask
    {
        event EventHandler<RobotDataReceivedEventArgs> DataReceived;
        event EventHandler<ReceiverErrorEventArgs> ErrorOccurred;

        void Start();
        void Stop();
    }
}