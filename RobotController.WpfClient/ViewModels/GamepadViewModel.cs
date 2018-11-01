using System;
using RobotController.Gamepad.Models;

namespace RobotController.WpfGui.ViewModels
{
    public class GamepadViewModel : ObservableEntity
    {
        private GamepadModel _gamepadModel;

        public GamepadModel GamepadModel
        {
            get { return _gamepadModel; }
            set
            {
                _gamepadModel = value;
                OnPropertyChanged(nameof(GamepadModel));               
            }
        }
    }
}
