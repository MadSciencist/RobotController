using RobotController.Communication.Interfaces;
using System;
using System.Diagnostics;
using System.Timers;
using RobotController.Communication.Extensions;

namespace RobotController.Communication.Utils
{
    internal class Watchdog : IWatchdog
    {
        public event EventHandler<EventArgs> TimeoutOccured;
        private readonly Timer _timer;

        public Watchdog(int timeout)
        {
            _timer = new Timer(timeout);
            _timer.Elapsed += OnElapsed;
            _timer.Start();
        }

        public void ResetWatchdog() => _timer.Reset();
        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Connection timeout: robot is not sending keep alive");
            TimeoutOccured?.Invoke(this, EventArgs.Empty);
        }
    }
}
