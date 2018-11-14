﻿using RobotController.Communication;
using RobotController.Communication.SerialStream;
using RobotController.Gamepad;
using System;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Configurations;
using NLog;
using RobotController.Communication.Enums;
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Gamepad.Config;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;
using RobotController.RobotModels;
using RobotController.WpfGui.Charts;
using RobotController.WpfGui.ExtendedControls;
using RobotController.WpfGui.ViewModels;

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
        private GamepadChart _chart;
        private SpeedFeedbackChart _speedFeedbackChart;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            _logger.Info("Created GUI instance");
            serialPortFactory = new SerialPortFactory();
            _chart = new GamepadChart();
            _speedFeedbackChart = new SpeedFeedbackChart();
            config = new SteeringConfig();
            _mainViewModel = new MainViewModel();
            gamepad = new GamepadController(config, 0, 25);
            gamepad.GamepadStateChanged += GamepadStateChanged;
            gamepad.RobotControlChanged += GamepadOnRobotControlChanged;
            gamepad.Start();

            _mainViewModel.FeedbackChartViewModel.SpeedFeedbackChart = _speedFeedbackChart;
            _mainViewModel.GamepadChartViewModel.GamepadChart = _chart;
            _mainViewModel.ControlSettingsViewModel.SteeringConfig = config;

            DataContext = _mainViewModel;
            LoadPortNames();
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                _mainViewModel.FeedbackChartViewModel.SpeedFeedbackChart.AddNewPoint(e.LeftMotor.RawSpeed, e.RightMotor.RawSpeed);
            });
        }

        private void ComboboxPortsOnDropdownOpened(object sender, EventArgs e) => LoadPortNames();
        private void LoadPortNames()
        {
            var ports = SerialPortUtils.GetAvailablePorts();
            if (ports.Length > 0)
            {
                var observable = new ObservableCollection<string>(ports);
                //PortsCombobox.ItemsSource = observable;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (robotConnection == null)
            {
                _logger.Info("Starting connection...");
                serialPort = serialPortFactory.GetPort("COM3");
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (robotConnection != null)
            {
                _logger.Info("Stopping connection...");
                robotConnection.Dispose();
                robotConnection = null;
            }

            if (serialPort != null)
            {
                if(serialPort.IsOpen) serialPort.DiscardInBuffer();
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
                    Payload = new byte[8]
                };

                robotConnection?.SendCommand(message, source.EPriority);
            }
        }
    }
}
