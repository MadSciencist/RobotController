using RobotController.Communication;
using RobotController.Communication.SerialStream;
using RobotController.Gamepad;
using System;
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

namespace RobotController.WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort;
        SerialPortAdapter serialPortAdapter;
        SerialPortFactory serialPortFactory;
        SerialPortManager serialPortManager;
        RobotConnectionFacade robotConnection;
        GamepadController gamepad;

        public MainWindow()
        {
            serialPortFactory = new SerialPortFactory();
            serialPortManager = new SerialPortManager();
            gamepad = new GamepadController(0, 50);

            InitializeComponent();
            LoadPortNames();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (robotConnection == null)
            {
                serialPort = serialPortFactory.GetPort("COM3");
                serialPortManager.TryOpen(serialPort);
                serialPortAdapter = new SerialPortAdapter(serialPort);
                robotConnection = new RobotConnectionFacade(serialPortAdapter);
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
    }
}
