using RobotController.Communication;
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
using RobotController.Communication.Interfaces;
using RobotController.Communication.Messages;
using RobotController.Gamepad.Config;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;
using RobotController.WpfGui.Charts;
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

        public MainWindow()
        {
            InitializeComponent();
            serialPortFactory = new SerialPortFactory();
            serialPortManager = new SerialPortManager();
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


        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

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
                serialPort = serialPortFactory.GetPort("COM3");
                serialPortManager.TryOpen(serialPort);
                serialPortAdapter = new SerialPortAdapter(serialPort);
                robotConnection = new RobotConnectionFacade(serialPortAdapter);
                robotConnection.FeedbackReceived += RobotConnectionOnFeedbackReceived;
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (robotConnection != null)
            {
                robotConnection.Dispose();
                robotConnection = null;
            }

            if (serialPort != null)
            {
                serialPort.DiscardInBuffer();
                serialPortManager.Close(serialPort);
                serialPort.Dispose();
                serialPort = null;
            }
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    BtnDisconnect_Click(this, null);
        //    base.OnClosing(e);
        //}
    }
}
