using NLog;
using RobotController.Communication;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.SerialStream;
using RobotController.DataLogger;
using RobotController.Gamepad;
using RobotController.Gamepad.Config;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;
using RobotController.RobotModels;
using RobotController.WpfGui.BusinessLogic;
using RobotController.WpfGui.Charts;
using RobotController.WpfGui.ExtendedControls;
using RobotController.WpfGui.Infrastructure;
using RobotController.WpfGui.Models;
using RobotController.WpfGui.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace RobotController.WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;
        private IStreamResource _serialPortAdapter;
        private readonly ISerialPortFactory _serialPortFactory;
        private SerialPortManager _serialPortManager;
        private RobotConnectionService _robotConnectionService;
        private ControlsSender _sender;

        private IGamepadService _gamepadService;

        private MainViewModel _mainViewModel;
        private SteeringConfig _steeringConfig;

        private DataLoggerService _dataLoggerService;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            Logger.Info("Created GUI instance");
            _serialPortFactory = new SerialPortFactory();

            CreateGamepadService();
            CreateMainViewModel();
            LoadPortNames();

            CreateDataLogger(new LogConfig { Path = @".\default_log.csv" });

            _gamepadService.Start();

            DataContext = _mainViewModel;
        }

        private void GamepadService_SteeringPointChanged(object sender, Point point)
        {
            _mainViewModel.GamepadChart.UpdateLivePointChart(point);
        }

        private void ControlSettings_OnExpoSliderChanged(object sender, short e)
        {
            _steeringConfig.ExponentialCurveCoefficient = e;
            var expo = _gamepadService.UpdateExponentialCurve(e);

            _mainViewModel.GamepadChart.UpdateExpoChart(expo);
        }

        private void GamepadSerivce_RobotControlChanged(object sender, RobotControlEventArgs e)
        {
            _mainViewModel.RobotControlsViewModel.RobotControl = e.RobotControl;
            _sender?.UpdateAndSendControls(e.RobotControl);
        }

        private void GamepadService_GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            _mainViewModel.GamepadViewModel.GamepadModel = e.GamepadModel;
        }

        private void RobotConnection_CurrentSpeedFeedbackReceived(object sender, MessageParsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                var now = DateTime.Now;

                if (_mainViewModel.GuiStatusViewModel.IsRawVelocityEnabled)
                {
                    _mainViewModel.SpeedFeedbackChart.AddNewPoint(
                        new MeasurementModel {DateTime = now, Value = e.LeftMotor.RawVelocity},
                        new MeasurementModel {DateTime = now, Value = e.RightMotor.RawVelocity});
                }
                else
                {
                    _mainViewModel.SpeedFeedbackChart.AddNewPoint(
                        new MeasurementModel { DateTime = now, Value = e.LeftMotor.Velocity },
                        new MeasurementModel { DateTime = now, Value = e.RightMotor.Velocity });
                }

                if (_mainViewModel.GuiStatusViewModel.IsRawCurrentEnabled)
                {
                    _mainViewModel.CurrentFeedbackChart.AddNewPoint(
                        new MeasurementModel {DateTime = now, Value = e.LeftMotor.RawCurrent},
                        new MeasurementModel {DateTime = now, Value = e.RightMotor.RawCurrent });
                }
                else
                {
                    _mainViewModel.CurrentFeedbackChart.AddNewPoint(
                        new MeasurementModel { DateTime = now, Value = e.LeftMotor.Current },
                        new MeasurementModel { DateTime = now, Value = e.RightMotor.Current });
                }
            }));
        }

        // done
        private void RobotConnection_VoltageTemperatureFeedbackReceived(object sender, MessageParsedEventArgs e) =>
            _mainViewModel.RobotControlsViewModel.RobotStatus = new RobotStatusModel
            {
                Temperature = e.VoltageTemperatureFeedbackModel.Temperature,
                Voltage = e.VoltageTemperatureFeedbackModel.Voltage
            };

        // done
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

               TrySend(message, source.EPriority);
            }
        }

        // done
        private void RobotSettings_OnTextBoxEnterPressed(object sender, SendingTextBoxEventArgs e)
        {
            if (sender is ExtendedTexBbox source)
            {
                try
                {
                    var message = new SendMessage
                    {
                        CommandType = source.ECommand,
                        Node = source.ENode,
                        Payload = TypeCaster.Cast(e.Value, source.EType)
                    };

                    TrySend(message, source.EPriority);
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

                TrySend(message, source.EPriority);
            }
        }

        private void TrySend(ISendMessage message, EPriority priority)
        {
            if (_sender == null)
            {
                MessageBox.Show("Please connect first", "Connection error");
                return;
            }

            _sender.SendMessage(message, priority);
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

                TrySend(message, source.EPriority);
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
            _mainViewModel = new MainViewModel
            {
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

                if (string.IsNullOrEmpty(portName))
                {
                    Logger.Error("No ports found");
                    MessageBox.Show("Please seelect serial port first.", "Port not found");
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

                _sender = new ControlsSender(_robotConnectionService, 50);

                RequestParameters();
            }
        }

        private void RequestParameters()
        {
            var message = new SendMessage
            {
                CommandType = ESenderCommand.Hello,
                Node = ENode.Master,
                Payload = Convert.ToByte(0)
            };

            TrySend(message, EPriority.VeryHigh);
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

        private void Status_OnExportClicked(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream("settings.xml", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var serializer = new XmlSerializer(typeof(ParametersModel));
                serializer.Serialize(stream, _mainViewModel.RobotControlsViewModel.ParametersModel);
            }
        }

        private void Status_OnImportClicked(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream("settings.xml", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var serializer = new XmlSerializer(typeof(ParametersModel));
                _mainViewModel.RobotControlsViewModel.ParametersModel = (ParametersModel)serializer.Deserialize(stream);
            }
        }
    }
}