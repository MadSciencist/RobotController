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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using RobotController.DataLogger;
using RobotController.WpfGui.Controls;
using RobotController.WpfGui.Infrastructure;

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

        //these 2 needs to be cleaned up
        private IList<MeasurementModel> left;
        private IList<MeasurementModel> right;

        private DispatcherTimer _dispatcherTimer;
        private DataLoggerService _dataLoggerService;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            Logger.Info("Created GUI instance");
            _serialPortFactory = new SerialPortFactory();

            CreateGamepadService();

            left = new List<MeasurementModel>();
            right = new List<MeasurementModel>();

            CreateMainViewModel();

            LoadPortNames();
            CreateDispatcherTimer();
            CreateDataLogger(new LogConfig { Path = @".\default_log.csv" });

            _gamepadService.Start();
            DataContext = _mainViewModel;
        }

        private void GamepadService_SteeringPointChanged(object sender, Point point) =>
            _mainViewModel.GamepadChart.UpdateLivePointChart(point);

        private void ControlSettings_OnExpoSliderChanged(object sender, short e)
        {
            _steeringConfig.ExponentialCurveCoefficient = e;
            var expo = _gamepadService.UpdateExponentialCurve(e);

            _mainViewModel.GamepadChart.UpdateExpoChart(expo);
        }

        private void Timer_OnDispatcherTimerTick(object sender, EventArgs e)
        {
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
                    Logger.Error(ex, "Error while parsing input");
                }
                catch (OverflowException ex)
                {
                    Logger.Error(ex, "Error while parsing input");
                }
            }
        }



        private void RobotSettings_OnTextBoxEnterPressedNew(object sender, SendingTextBoxEventArgs e)
        {
            if (sender is ExtendedTexBbox source)
            {
                try
                {
                    var message = new SendMessage
                    {
                        CommandType = source.ECommand,
                        Node = source.ENode,
                        Payload = TypeCaster.Cast(e.Value.ToString(), source.EType)
                    };

                    _robotConnectionService?.SendCommand(message, source.EPriority);
                }
                catch (FormatException ex)
                {
                    Logger.Error(ex, "Error while parsing input");
                }
                catch (OverflowException ex)
                {
                    Logger.Error(ex, "Error while parsing input");
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
        private void LoadPortNames()
        {
            var ports = SerialPortUtils.GetAvailablePorts().OrderByDescending(x => x);
            _mainViewModel.GuiStatusViewModel.AvailablePorts = new List<string>(ports);
        }


        private void WindowClose(object sender, CancelEventArgs e)
        {
            _gamepadService.Stop();

            if (_robotConnectionService != null)
            {
                Logger.Info("Stopping connection...");
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
        private void CreateDataLogger(ILogConfig config)
        {
            _dataLoggerService = new DataLoggerService(config);
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

        private void Navbar_OnDisconnectButtonClicked(object sender, RoutedEventArgs e)
        {
            _mainViewModel.GuiStatusViewModel.ConnectionStatus = "Disconnected";
            _mainViewModel.GuiStatusViewModel.IsConnected = false;

            if (_robotConnectionService != null)
            {
                Logger.Info("Stopping connection...");
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

        private void Navbar_OnConnectButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_robotConnectionService == null)
            {
                var portName = _selectedPortName;
                if (portName == string.Empty)
                {
                    Logger.Error("No ports found");
                    return;
                }

                Logger.Info("Starting connection...");
                _mainViewModel.GuiStatusViewModel.ConnectionStatus = $"Connected to: {portName}";
                _mainViewModel.GuiStatusViewModel.IsConnected = true;
                _serialPort = _serialPortFactory.GetPort(portName);
                _serialPortManager = new SerialPortManager(_serialPort);
                _serialPortManager.TryOpen();
                _serialPortAdapter = new SerialPortAdapter(_serialPort);
                _robotConnectionService = new RobotConnectionService(_serialPortAdapter);
                _robotConnectionService.SpeedCurrentFeedbackReceived += RobotConnection_CurrentSpeedFeedbackReceived;
                _robotConnectionService.VoltageTemperatureFeedbackReceived += RobotConnection_VoltageTemperatureFeedbackReceived;
                _robotConnectionService.ParametersReceived += RobotConnection_ParametersReceived;
            }
        }

        private void Navbar_OnSerialPortDropDownOpened(object sender, EventArgs e) => LoadPortNames();

        private string _selectedPortName;
        private void Navbar_OnSelectedPortChanged(object sender, string e) => _selectedPortName = e;

        private void Navbar_OnStartLoggingClicked(object sender, RoutedEventArgs e) =>
            _dataLoggerService.SubscribeAndStart(_robotConnectionService, _gamepadService);

        private void Navbar_OnStopLoggingClicked(object sender, RoutedEventArgs e) =>
            _dataLoggerService.UnSubscribeAndStop();

        private void Navbar_OnLoggingPathChanged(object sender, RoutedEventArgs e)
        {
            var dialog = new DataLogSaveDialog();
            var path = dialog.SelectDatalogPath();

            CreateDataLogger(new LogConfig { Path = path });
        }

        private void ControlSettings_OnLowPassFilterSliderChanged(object sender, short e)
        {
        }

    }
}