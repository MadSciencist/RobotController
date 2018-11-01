using System;

namespace RobotController.Gamepad.Interfaces
{
    public interface IGamepadController
    {
        event EventHandler<GamepadEventArgs> GamepadStateChanged;
        void Start();
        void Stop();
    }
}
