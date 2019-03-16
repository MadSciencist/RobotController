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

        /// <summary>
        /// This will fire when X button is clicked
        /// </summary>
        event EventHandler AllowFullSpeedClicked;

        /// <summary>
        /// This will fire when A button is clicked
        /// </summary>
        event EventHandler LimitSpeedClicked;

        /// <summary>
        /// This will fire when B button is clicked
        /// </summary>
        event EventHandler StopClicked;

        /// <summary>
        /// This will fire when START button is clicked
        /// </summary>
        event EventHandler StartClicked;

        IList<short> UpdateExponentialCurve(short coefficient);

        void Start();
        void Stop();
    }
}
