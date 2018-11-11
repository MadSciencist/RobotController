﻿using System;
using System.Timers;

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

        public void ResetWatchdog()
        {
            _timer.Stop();
            _timer.Start();
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            TimeoutOccured?.Invoke(this, EventArgs.Empty);
        }
    }
}
