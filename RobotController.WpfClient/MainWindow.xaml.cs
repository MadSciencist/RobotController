using NLog;
using RobotController.Communication;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.SerialStream;
using RobotController.Gamepad;
using RobotController.Gamepad.Config;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;
using RobotController.WpfGui.BusinessLogic;
using RobotController.WpfGui.Charts;
using RobotController.WpfGui.ExtendedControls;
using RobotController.WpfGui.Models;
using RobotController.WpfGui.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using RobotController.DataLogger;

namespace RobotController.WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;
        private IStreamResource _serialPortAdapter;
        private ISerialPortFactory _serialPortFactory;
        private SerialPortManager _serialPortManager;
        private RobotConnectionService _robotConnectionService;

        private IGamepadService _gamepadService;

        private MainViewModel _mainViewModel;
        private GamepadChart _gamepadChart;
        private SpeedFeedbackChart _speedFeedbackChart;
        private SteeringConfig _steeringConfig;

        //these 3 needs to be cleaned up
        private Point triggerPosition;
        private IList<MeasurementModel> left;
        private IList<MeasurementModel> right;

        private DispatcherTimer _dispatcherTimer;
        private DataLoggerService _dataLoggerService;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            _logger.Info("Created GUI instance");
            _serialPortFactory = new SerialPortFactory();

            CreateGamepadService();

            triggerPosition = new Point();
            left = new List<MeasurementModel>();
            right = new List<MeasurementModel>();

            CreateMainViewModel();

            LoadPortNames();
            CreateDispatcherTimer();
            CreateDataLogger();

            _gamepadService.Start();
            DataContext = _mainViewModel;
        }

        private void GamepadService_SteeringPointChanged(object sender, Point e)
        {
            triggerPosition = e;
        }

        private void ControlSettings_OnExpoSliderChanged(object sender, short e)
        {
            _steeringConfig.ExponentialCurveCoefficient = e;
            var expo = _gamepadService.UpdateExponentialCurve(e);

            _mainViewModel.GamepadChart.UpdateExpoChart(expo);
        }

        private void FilterSliderChanged(object sender, short e) { }

        private void Timer_OnDispatcherTimerTick(object sender, EventArgs e)
        {
            _mainViewModel.GamepadChart.UpdateLivePointChart(triggerPosition);

            Application.Current.Dispatcher.Invoke((Action)(() =>
           {
               _mainViewModel.SpeedFeedbackChart.AddNewPoints(left, right);

           }));

            left.Clear();
            right.Clear();
        }


        private void GamepadSerivce_RobotControlChanged(object sender, RobotControlEventArgs e)
        {
            _mainViewModel.RobotControlsViewModel.RobotControl = e.RobotControl;
        }

        private void GamepadService_GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            _mainViewModel.GamepadViewModel.GamepadModel = e.GamepadModel;
        }

        private void RobotConnection_CurrentSpeedFeedbackReceived(object sender, MessageParsedEventArgs e)
        {
            left.Add(new MeasurementModel { DateTime = DateTime.Now, Value = e.LeftMotor.Velocity });
            right.Add(new MeasurementModel { DateTime = DateTime.Now, Value = e.RightMotor.Velocity });
        }

        //done
        private void RobotConnection_VoltageTemperatureFeedbackReceived(object sender, MessageParsedEventArgs e) =>
            _mainViewModel.RobotControlsViewModel.RobotStatus = new RobotStatusModel
            {
                Temperature = e.VoltageTemperatureFeedbackModel.Temperature,
                Voltage = e.VoltageTemperatureFeedbackModel.Voltage
            };

        //done
        private void RobotConnection_ParametersReceived(object sender, MessageParsedEventArgs e) =>
            _mainViewModel.RobotControlsViewModel.ParametersModel = e.Parameters;

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (_robotConnectionService == null)
            {
                var portName = PortComboBox.Text;
                if (portName == string.Empty)
                {
                    _logger.Error("No ports found");
                    return;
                }

                _logger.Info("Starting connection...");
                _mainViewModel.GuiStatusViewModel.ConnectionStatus = $"Connected to: {portName}";
                _mainViewModel.GuiStatusViewModel.IsConnected = true;
                _serialPort = _serialPortFactory.GetPort(PortComboBox.Text);
                _serialPortManager = new SerialPortManager(_serialPort);
                _serialPortManager.TryOpen();
                _serialPortAdapter = new SerialPortAdapter(_serialPort);
                _robotConnectionService = new RobotConnectionService(_serialPortAdapter);
                _robotConnectionService.SpeedCurrentFeedbackReceived += RobotConnection_CurrentSpeedFeedbackReceived;
                _robotConnectionService.VoltageTemperatureFeedbackReceived += RobotConnection_VoltageTemperatureFeedbackReceived;
                _robotConnectionService.ParametersReceived += RobotConnection_ParametersReceived;
            }
        }

        private void DisconnectButtonClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.GuiStatusViewModel.ConnectionStatus = "Disconnected";
            _mainViewModel.GuiStatusViewModel.IsConnected = false;

            if (_robotConnectionService != null)
            {
                _logger.Info("Stopping connection...");
                _robotConnectionService.Dispose();
                _robotConnectionService = null;
            }

            if (_serialPort != null)
            {
                _serialPortManager.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is ExtendedButton source)
            {
                var message = new SendMessage
                {
                    CommandType = source.ECommand,
                    Node = source.ENode,
                    Payload = (byte)0x00
                };

                _robotConnectionService?.SendCommand(message, source.EPriority);
            }
        }

        //done
        private void RobotSettings_TextBoxEnterPressed(object sender, KeyEventArgs e)
        {
            if (sender is ExtendedTexBbox source)
            {
                try
                {
                    var message = new SendMessage
                    {
                        CommandType = source.ECommand,
                        Node = source.ENode,
                        Payload = TypeCaster.Cast(source.Text, source.EType)
                    };

                    _robotConnectionService?.SendCommand(message, source.EPriority);
                }
                catch (FormatException ex)
                {
                    _logger.Error(ex, "Error while parsing input");
                }
                catch (OverflowException ex)
                {
                    _logger.Error(ex, "Error while parsing input");
                }
            }
        }

        //done
        private void RobotSettings_RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is ExtentedRadioButton source)
            {
                var message = new SendMessage
                {
                    CommandType = source.ESenderCommand,
                    Node = source.ENode,
                    Payload = source.State
                };

                _robotConnectionService?.SendCommand(message, source.EPriority);
            }
        }

        //done
        private void RobotSettings_OnCheckboxChanged(object sender, RoutedEventArgs e)
        {
            if (sender is ExtentedCheckBox source)
            {
                var message = new SendMessage
                {
                    CommandType = source.ECommand,
                    Node = source.ENode,
                    Payload = Convert.ToByte(source.IsChecked)
                };

                Console.WriteLine(message.Payload);
                _robotConnectionService?.SendCommand(message, source.EPriority);
            }
        }

        //done
        private void OnSerialPortDropDownOpened(object sender, EventArgs e) => LoadPortNames();

        //done
        private void LoadPortNames()
        {
            var ports = SerialPortUtils.GetAvailablePorts();
            if (ports.Length <= 0) return;
            var observable = new ObservableCollection<string>(ports);
            PortComboBox.ItemsSource = observable;
            PortComboBox.Text = observable[0];
        }

      
        private void WindowClose(object sender, CancelEventArgs e)
        {
            _gamepadService.Stop();

            if (_robotConnectionService != null)
            {
                _logger.Info("Stopping connection...");
                _robotConnectionService.Dispose();
                _robotConnectionService = null;
            }

            if (_serialPort != null)
            {
                if (_serialPort.IsOpen) _serialPort.DiscardInBuffer();
                _serialPortManager.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        //done
        private void CreateMainViewModel()
        {
            _gamepadChart = new GamepadChart();
            _speedFeedbackChart = new SpeedFeedbackChart();
            _mainViewModel = new MainViewModel
            {
                SpeedFeedbackChart = _speedFeedbackChart,
                GamepadChart = _gamepadChart,
                ControlSettingsViewModel = { SteeringConfig = _steeringConfig }
            };
        }

        //done
        private void CreateGamepadService()
        {
            _steeringConfig = new SteeringConfig();
            _gamepadService = new GamepadService(_steeringConfig, 0, 40);
            _gamepadService.GamepadStateChanged += GamepadService_GamepadStateChanged;
            _gamepadService.RobotControlChanged += GamepadSerivce_RobotControlChanged;
            _gamepadService.SteeringPointChanged += GamepadService_SteeringPointChanged;
        }

        //done
        private void CreateDispatcherTimer()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += Timer_OnDispatcherTimerTick;
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
            _dispatcherTimer.Start();
        }

        //done
        private void StartLoggingButton_OnClick(object sender, RoutedEventArgs e) =>
            _dataLoggerService.SubscribeAndStart(_robotConnectionService, _gamepadService);

        //done
        private void StopLoggingButton_OnClick(object sender, RoutedEventArgs e) =>
            _dataLoggerService.UnSubscribeAndStop();

        //done
        private void CreateDataLogger()
        {
            _dataLoggerService = new DataLoggerService(new LogConfig { Path = @".\" });
            _dataLoggerService.LoggingStarted += (sender, args) =>
            {
                _mainViewModel.GuiStatusViewModel.IsLogging = true;
                _mainViewModel.GuiStatusViewModel.LoggingStatus = "Logging active!";
            };

            _dataLoggerService.LoggingStopped += (sender, args) =>
            {
                _mainViewModel.GuiStatusViewModel.IsLogging = false;
                _mainViewModel.GuiStatusViewModel.LoggingStatus = "Logging stopped";
            };
        }
    }
}
