using System;

namespace RobotController.Communication.Interfaces
{
    internal interface IWatchdog
    {
        event EventHandler<EventArgs> TimeoutOccured;
        void ResetWatchdog();
        void Start();
        void Stop();
    }
}