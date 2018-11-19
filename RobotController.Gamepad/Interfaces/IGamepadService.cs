using RobotController.Gamepad.EventArguments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RobotController.Gamepad.Interfaces
{
    public interface IGamepadService
    {
        event EventHandler<GamepadEventArgs> GamepadStateChanged;
        event EventHandler<RobotControlEventArgs> RobotControlChanged;
        event EventHandler<Point> SteeringPointChanged;
        event EventHandler<ErrorEventArgs> GamepadErrorOccured;

        IList<short> UpdateExponentialCurve(short coefficient);

        void Start();
        void Stop();
    }
}
