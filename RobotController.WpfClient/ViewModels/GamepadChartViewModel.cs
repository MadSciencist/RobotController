using RobotController.WpfGui.Charts;

namespace RobotController.WpfGui.ViewModels
{
    public class GamepadChartViewModel : ObservableEntity
    {
        private GamepadChart _gamepadChart;

        public GamepadChart GamepadChart
        {
            get { return _gamepadChart; }
            set
            {
                _gamepadChart = value;
                OnPropertyChanged(nameof(GamepadChart));
            }
        }
    }
}
