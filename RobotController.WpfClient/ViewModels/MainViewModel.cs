using RobotController.RobotModels;
using RobotController.WpfGui.Charts;
using RobotController.WpfGui.Models;

namespace RobotController.WpfGui.ViewModels
{
    public class MainViewModel
    {
        public GamepadViewModel GamepadViewModel { get; private set; }
        public RobotControlsViewModel RobotControlsViewModel { get; private set; }
        public ControlSettingsViewModel ControlSettingsViewModel { get; set; }
        public GamepadChart GamepadChart { get; set; }
        public SpeedFeedbackChart SpeedFeedbackChart { get; set; }
        public ParametersViewModel ParametersViewModel { get; set; }


        public MainViewModel()
        {
            GamepadChart = new GamepadChart();
            SpeedFeedbackChart = new SpeedFeedbackChart();
            GamepadViewModel = new GamepadViewModel();
            RobotControlsViewModel = new RobotControlsViewModel { RobotStatus = new RobotStatusModel() };
            ControlSettingsViewModel = new ControlSettingsViewModel();
            ParametersViewModel = new ParametersViewModel();
        }
    }

    public class ParametersViewModel : ObservableEntity
    {
        private ParametersModel _parameters;

        public ParametersModel ParametersModel
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                OnPropertyChanged(nameof(ParametersModel));
            }
        }
    }
}
