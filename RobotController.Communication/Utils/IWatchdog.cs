using System;

namespace RobotController.Communication.Utils
{
    internal interface IWatchdog
    {
        event EventHandler<EventArgs> TimeoutOccured;
        void ResetWatchdog();
        void Start();
        void Stop();
    }
}