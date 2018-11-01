using RobotController.Gamepad.Models;
using System;

namespace RobotController.Gamepad
{
    public class GamepadEventArgs : EventArgs
    {
        public GamepadModel GamepadModel  { get; set; }
        public RobotControlModel RobotControl { get; set; }
    }
}
