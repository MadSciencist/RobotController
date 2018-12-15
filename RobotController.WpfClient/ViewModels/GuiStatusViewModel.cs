using System.Collections.Generic;

namespace RobotController.WpfGui.ViewModels
{
    public class GuiStatusViewModel : ObservableEntity
    {
        private IEnumerable<string> _availablePorts;
        public IEnumerable<string> AvailablePorts
        {
            get { return _availablePorts; }
            set
            {
                _availablePorts = value;
                OnPropertyChanged(nameof(AvailablePorts));
            }
        }


        private string _connectionStatus = "Disconnected";
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        private string _loggingStatus = "Logger stopped";
        public string LoggingStatus
        {
            get { return _loggingStatus; }
            set
            {
                _loggingStatus = value;
                OnPropertyChanged(nameof(LoggingStatus));
            }
        }

        private bool _isLogging = false;
        public bool IsLogging
        {
            get { return _isLogging; }
            set
            {
                _isLogging = value;
                OnPropertyChanged(nameof(IsLogging));
            }
        }
    }
}