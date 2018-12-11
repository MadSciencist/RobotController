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
        private SerialPort serialPort;
        private IStreamResource serialPortAdapter;
        private ISerialPortFactory serialPortFactory;
        private SerialPortManager serialPortManager;
        private RobotConnectionService robotConnection;
        private IGamepadService gamepad;
        private MainViewModel _mainViewModel;
        private SteeringConfig config;
        private GamepadChart _gamepadChart;
        private SpeedFeedbackChart _speedFeedbackChart;

        private IList<MeasurementModel> left;
        private IList<MeasurementModel> right;
        private DispatcherTimer dispatcherTimer;
        private DataLoggerService _dataLoggerService;

        private Point triggerPosition;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            _logger.Info("Created GUI instance");
            serialPortFactory = new SerialPortFactory();
            _gamepadChart = new GamepadChart();
            _speedFeedbackChart = new SpeedFeedbackChart();
            config = new SteeringConfig();
            _mainViewModel = new MainViewModel();
            gamepad = new GamepadService(config, 0, 40);
            gamepad.GamepadStateChanged += GamepadService_GamepadStateChanged;
            gamepad.RobotControlChanged += GamepadSerivce_RobotControlChanged;
            gamepad.SteeringPointChanged += GamepadService_SteeringPointChanged;
            gamepad.Start();

            triggerPosition = new Point();

            _mainViewModel.SpeedFeedbackChart = _speedFeedbackChart;
            _mainViewModel.GamepadChart = _gamepadChart;
            _mainViewModel.ControlSettingsViewModel.SteeringConfig = config;

            DataContext = _mainViewModel;
            LoadPortNames();

            left = new List<MeasurementModel>();
            right = new List<MeasurementModel>();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnDispatcherTimerTick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
            dispatcherTimer.Start();
            _dataLoggerService = new DataLoggerService(robotConnection, gamepad, null);
        }

        private void GamepadService_SteeringPointChanged(object sender, Point e)
        {
            triggerPosition = e;
        }

        private void ControlSettings_OnExpoSliderChanged(object sender, short e)
        {
            config.ExponentialCurveCoefficient = e;
            var expo = gamepad.UpdateExponentialCurve(e);

            _mainViewModel.GamepadChart.UpdateExpoChart(expo);
        }

        private void FilterSliderChanged(object sender, short e){}

        private void OnDispatcherTimerTick(object sender, EventArgs e)
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
            left.Add(new MeasurementModel { DateTime = DateTime.Now, Value = e.LeftMotor.RawSpeed });
            right.Add(new MeasurementModel { DateTime = DateTime.Now, Value = e.RightMotor.RawSpeed });
        }

        private void RobotConnection_VoltageTemperatureFeedbackReceived(object sender, MessageParsedEventArgs e)
        {
            _mainViewModel.RobotControlsViewModel.RobotStatus = new RobotStatusModel
            {
                Temperature = e.VoltageTemperatureFeedbackModel.RawTemperature,
                Voltage = e.VoltageTemperatureFeedbackModel.RawVoltage
            };
        }

        private void RobotConnection_ParametersReceived(object sender, MessageParsedEventArgs e)
        {
            _mainViewModel.ParametersViewModel.ParametersModel = e.Parameters;
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (robotConnection == null)
            {
                var portName = PortComboBox.Text;
                if (portName == string.Empty)
                {
                    _logger.Error("No ports found");
                    return;
                }

                _logger.Info("Starting connection...");
                serialPort = serialPortFactory.GetPort(PortComboBox.Text);
                serialPortManager = new SerialPortManager(serialPort);
                serialPortManager.TryOpen();
                serialPortAdapter = new SerialPortAdapter(serialPort);
                robotConnection = new RobotConnectionService(serialPortAdapter);
                robotConnection.SpeedCurrentFeedbackReceived += RobotConnection_CurrentSpeedFeedbackReceived;
                robotConnection.VoltageTemperatureFeedbackReceived += RobotConnection_VoltageTemperatureFeedbackReceived;
                robotConnection.ParametersReceived += RobotConnection_ParametersReceived;
            }
        }

        private void DisconnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (robotConnection != null)
            {
                _logger.Info("Stopping connection...");
                robotConnection.Dispose();
                robotConnection = null;
            }

            if (serialPort != null)
            {
                serialPortManager.Close();
                serialPort.Dispose();
                serialPort = null;
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

                robotConnection?.SendCommand(message, source.EPriority);
            }
        }


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

                    robotConnection?.SendCommand(message, source.EPriority);
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

                robotConnection?.SendCommand(message, source.EPriority);
            }
        }

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
                robotConnection?.SendCommand(message, source.EPriority);
            }
        }


        private void OnSerialPortDropDownOpened(object sender, EventArgs e) => LoadPortNames();

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
            gamepad.Stop();

            if (robotConnection != null)
            {
                _logger.Info("Stopping connection...");
                robotConnection.Dispose();
                robotConnection = null;
            }

            if (serialPort != null)
            {
                if (serialPort.IsOpen) serialPort.DiscardInBuffer();
                serialPortManager.Close();
                serialPort.Dispose();
                serialPort = null;
            }
        }
    }
}
