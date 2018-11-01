using RobotController.Gamepad.Models;

namespace RobotController.WpfGui.ViewModels
{
    public class RobotControlsViewModel : ObservableEntity
    {
        private RobotControlModel _robotControlModel;

        public RobotControlModel RobotControl
        {
            get { return _robotControlModel; }
            set
            {
                _robotControlModel = value;
                OnPropertyChanged(nameof(RobotControl));
            }
        }
    }
}
