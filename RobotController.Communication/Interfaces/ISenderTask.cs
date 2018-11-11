using RobotController.Communication.SendingTask;
using System;

namespace RobotController.Communication.Interfaces
{
    internal interface ISenderTask
    {
        event EventHandler<SenderErrorEventArgs> ErrorOccurred;
        void Start();
        void Stop();
    }
}