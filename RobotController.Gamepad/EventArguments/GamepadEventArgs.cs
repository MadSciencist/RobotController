using System;
using RobotController.Gamepad.Models;

namespace RobotController.Gamepad.EventArguments
{
    public class GamepadEventArgs : EventArgs
    {
        public GamepadModel GamepadModel  { get; set; }
    }
}
