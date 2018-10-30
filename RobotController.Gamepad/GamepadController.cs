using J2i.Net.XInputWrapper;
using System;

namespace RobotController.Gamepad
{
    public class GamepadController : IDisposable
    {
        private readonly XboxController _controller;

        // default update frequency
        public GamepadController(int controllerIndex) : this(controllerIndex, 25) { }

        public GamepadController(int controllerIndex, int updateFrequency)
        {
            if (updateFrequency <= 0) throw new ArgumentException();

            _controller = XboxController.RetrieveController(controllerIndex);
            _controller.StateChanged += StateChanged;
            XboxController.UpdateFrequency = updateFrequency;
            XboxController.StartPolling();
        }

        private void StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            Console.WriteLine(e.CurrentInputState.Gamepad.sThumbLX);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                XboxController.StopPolling();
            }
        }
    }
}
