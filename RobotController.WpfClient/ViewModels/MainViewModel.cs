using RobotController.Gamepad.Models;
using RobotController.WpfGui.Charts;

namespace RobotController.WpfGui.ViewModels
{
    public class MainViewModel
    {
        public GamepadViewModel GamepadViewModel { get; private set; }
        public RobotControlsViewModel RobotControlsViewModel { get; private set; } 
        public ControlSettingsViewModel  ControlSettingsViewModel { get; set; }
        public GamepadChart GamepadChart { get; set; }
        public SpeedFeedbackChart SpeedFeedbackChart { get; set; }

        public MainViewModel()
        {
            GamepadChart = new GamepadChart();
            SpeedFeedbackChart = new SpeedFeedbackChart();
            GamepadViewModel = new GamepadViewModel();
            RobotControlsViewModel = new RobotControlsViewModel();
            ControlSettingsViewModel = new ControlSettingsViewModel();
        }
    }
}
