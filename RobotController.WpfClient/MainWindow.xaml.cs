using NLog;
using RobotController.Communication;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Communication.SerialStream;
using RobotController.Gamepad;
using RobotController.Gamepad.Config;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;
using RobotController.RobotModels;
using RobotController.WpfGui.BusinessLogic;
using RobotController.WpfGui.Charts;
using RobotController.WpfGui.ExtendedControls;
using RobotController.WpfGui.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using RobotController.WpfGui.Controls;

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
        private RobotConnectionFacade robotConnection;
        private IGamepadController gamepad;
        private MainViewModel _mainViewModel;
        private SteeringConfig config;
        private GamepadChart _gamepadChart;
        private SpeedFeedbackChart _speedFeedbackChart;

        private IList<MeasurementModel> left;
        private IList<MeasurementModel> right;
        private DispatcherTimer dispatcherTimer;

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
            gamepad = new GamepadController(config, 0, 25);
            gamepad.GamepadStateChanged += GamepadStateChanged;
            gamepad.RobotControlChanged += GamepadOnRobotControlChanged;
            gamepad.SteeringPointChanged += GamepadOnSteeringPointChanged;
            gamepad.Start();

            triggerPosition = new Point();

            _mainViewModel.FeedbackChartViewModel.SpeedFeedbackChart = _speedFeedbackChart;
            _mainViewModel.GamepadChart = _gamepadChart;
            _mainViewModel.ControlSettingsViewModel.SteeringConfig = config;

            DataContext = _mainViewModel;
            LoadPortNames();

            left = new List<MeasurementModel>();
            right = new List<MeasurementModel>();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnDispatcherTimerTick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(33);
            dispatcherTimer.Start();
        }

        private void GamepadOnSteeringPointChanged(object sender, Point e)
        {
            triggerPosition = e;
        }

        private void FilterSliderChanged(object sender, short e)
        {
            config.ExponentialCurveCoefficient = e;
            var expo = gamepad.UpdateExponentialCurve(e);

            _mainViewModel.GamepadChart.UpdateExpoChart(expo);
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            _mainViewModel.GamepadChart.UpdateLivePointChart(triggerPosition);

            Application.Current.Dispatcher.Invoke((Action)(() =>
           {
               _mainViewModel.FeedbackChartViewModel.SpeedFeedbackChart.AddNewPoints(left, right);
              
           }));

            left.Clear();
            right.Clear();
        }


        private void GamepadOnRobotControlChanged(object sender, RobotControlEventArgs e)
        {
             _mainViewModel.RobotControlsViewModel.RobotControl = e.RobotControl;
        }

        private void GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            _mainViewModel.GamepadViewModel.GamepadModel = e.GamepadModel;
        }

        private void RobotConnectionOnFeedbackReceived(object sender, MessageParsedEventArgs e)
        {
            left.Add(new MeasurementModel { DateTime = DateTime.Now, Value = e.LeftMotor.RawSpeed });
            right.Add(new MeasurementModel { DateTime = DateTime.Now, Value = e.RightMotor.RawSpeed });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (robotConnection == null)
            {
                var portName = PortComboBox.Text;
                if (portName == string.Empty)
                {
                    _logger.Error("No ports found");
                    return;;
                }

                _logger.Info("Starting connection...");
                serialPort = serialPortFactory.GetPort(PortComboBox.Text);
                serialPortManager = new SerialPortManager(serialPort);
                serialPortManager.TryOpen();
                serialPortAdapter = new SerialPortAdapter(serialPort);
                robotConnection = new RobotConnectionFacade(serialPortAdapter);
                robotConnection.FeedbackReceived += RobotConnectionOnFeedbackReceived;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var pi = Math.PI;
            int integer = 25;

            var command = new SendMessage()
            {
                CommandType = ESenderCommand.KeepAlive,
                Node = ENode.Master,
                Payload = pi
            };

            var command1 = new SendMessage()
            {
                CommandType = ESenderCommand.KeepAlive,
                Node = ENode.Driver1,
                Payload = integer
            };

            var command2 = new SendMessage()
            {
                CommandType = ESenderCommand.Controls,
                Node = ENode.Driver2,
                Payload = new ControlsModel()
                {
                    LeftSpeed = 10,
                    RightSpeed = 10
                }
            };

            robotConnection?.SendCommand(command, EPriority.Normal);
            robotConnection?.SendCommand(command1, EPriority.Normal);
            robotConnection?.SendCommand(command2, EPriority.VeryHigh);
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

        private void TextBoxEnterPressed(object sender, KeyEventArgs e)
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
