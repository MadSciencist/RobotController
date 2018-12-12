using RobotController.Gamepad.Models;
using RobotController.RobotModels;
using RobotController.WpfGui.Models;

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

        private RobotStatusModel _robotStatusModel;
        public RobotStatusModel RobotStatus
        {
            get { return _robotStatusModel; }
            set
            {
                _robotStatusModel = value;
                OnPropertyChanged(nameof(RobotStatus));
            }
        }

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
