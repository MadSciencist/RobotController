using System;
using RobotController.Gamepad.EventArguments;

namespace RobotController.Gamepad.Interfaces
{
    public interface IGamepadController
    {
        event EventHandler<GamepadEventArgs> GamepadStateChanged;
        event EventHandler<GamepadEventArgs> RobotControlChanged;
        event EventHandler<GamepadErrorEventArgs> GamepadErrorOccured;

        void Start();
        void Stop();
    }
}
