using RobotController.Gamepad.Config;

namespace RobotController.WpfGui.ViewModels
{
    public class ControlSettingsViewModel : ObservableEntity
    {
        private SteeringConfig _steeringConfig;

        public SteeringConfig SteeringConfig
        {
            get { return _steeringConfig; }
            set
            {
                _steeringConfig = value;
                OnPropertyChanged(nameof(SteeringConfig));
            }
        }
    }
}
