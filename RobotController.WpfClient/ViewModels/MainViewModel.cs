using RobotController.Gamepad.Models;
using RobotController.WpfGui.Charts;

namespace RobotController.WpfGui.ViewModels
{
    public class MainViewModel
    {
        public GamepadViewModel GamepadViewModel { get; private set; }
        public RobotControlsViewModel RobotControlsViewModel { get; private set; }
        //public GamepadChartViewModel GamepadChartViewModel { get; private set; }
        public FeedbackChartViewModel FeedbackChartViewModel { get; private set; }  
        public ControlSettingsViewModel  ControlSettingsViewModel { get; set; }
        public GamepadChart GamepadChart { get; set; }

        public MainViewModel()
        {
            GamepadChart = new GamepadChart();
            GamepadViewModel = new GamepadViewModel();
            //GamepadChartViewModel = new GamepadChartViewModel();
            RobotControlsViewModel = new RobotControlsViewModel();
            ControlSettingsViewModel = new ControlSettingsViewModel();
            FeedbackChartViewModel = new FeedbackChartViewModel();
        }
    }
}
