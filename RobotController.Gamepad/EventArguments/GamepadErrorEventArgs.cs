using System;
using System.IO;

namespace RobotController.Gamepad.EventArguments
{
    public class GamepadErrorEventArgs : ErrorEventArgs
    {
        public GamepadErrorEventArgs(Exception e) : base(e)
        {
        }
    }
}
