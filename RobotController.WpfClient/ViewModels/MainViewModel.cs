using RobotController.Gamepad.Models;

namespace RobotController.WpfGui.ViewModels
{
    public class MainViewModel
    {
        public GamepadViewModel GamepadViewModel { get; private set; }
        public RobotControlsViewModel RobotControlsViewModel { get; private set; }
        public GamepadChartViewModel GamepadChartViewModel { get; private set; }
        public ControlSettingsViewModel  ControlSettingsViewModel { get; set; }

        public MainViewModel()
        {
            GamepadViewModel = new GamepadViewModel();
            GamepadChartViewModel = new GamepadChartViewModel();
            RobotControlsViewModel = new RobotControlsViewModel();
            ControlSettingsViewModel = new ControlSettingsViewModel();
        }
    }
}
