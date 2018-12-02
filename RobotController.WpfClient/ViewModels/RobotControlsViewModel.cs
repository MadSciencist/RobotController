using RobotController.Gamepad.Models;
using RobotController.WpfGui.Models;

namespace RobotController.WpfGui.ViewModels
{
    public class RobotControlsViewModel : ObservableEntity
    {
        private RobotControlModel _robotControlModel;
        private RobotStatusModel _robotStatusModel;

        public RobotControlModel RobotControl
        {
            get { return _robotControlModel; }
            set
            {
                _robotControlModel = value;
                OnPropertyChanged(nameof(RobotControl));
            }
        }

        public RobotStatusModel RobotStatus
        {
            get { return _robotStatusModel; }
            set
            {
                _robotStatusModel = value;
                OnPropertyChanged(nameof(RobotStatus));
            }
        }
    }
}
