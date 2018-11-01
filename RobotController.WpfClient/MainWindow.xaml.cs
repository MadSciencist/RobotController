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
using RobotController.Communication.Interfaces;
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

        public MainWindow()
        {
            InitializeComponent();
            serialPortFactory = new SerialPortFactory();
            serialPortManager = new SerialPortManager();


            var chart = new GamepadChart();
            _mainViewModel = new MainViewModel();
            gamepad = new GamepadController(0, 25);
            gamepad.GamepadStateChanged += GamepadStateChanged;
            gamepad.Start();
            _mainViewModel.GamepadChartViewModel.GamepadChart = chart;
           

            DataContext = _mainViewModel;

           
            LoadPortNames();
        }

        private void GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            _mainViewModel.GamepadViewModel.GamepadModel = e.GamepadModel;
            _mainViewModel.RobotControlsViewModel.RobotControl = e.RobotControl;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (robotConnection == null)
            {
                serialPort = serialPortFactory.GetPort("COM3");
                serialPortManager.TryOpen(serialPort);
                serialPortAdapter = new SerialPortAdapter(serialPort);
                //robotConnection = new RobotConnectionFacade(serialPortAdapter);
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
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

        private void ComboboxPortsOnDropdownOpened(object sender, EventArgs e) => LoadPortNames();
        private void LoadPortNames()
        {
            var ports = SerialPortUtils.GetAvailablePorts();
            if (ports.Length > 0)
            {
                var observable = new ObservableCollection<string>(ports);
                PortsCombobox.ItemsSource = observable;
            }
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    BtnDisconnect_Click(this, null);
        //    base.OnClosing(e);
        //}
    }
}
