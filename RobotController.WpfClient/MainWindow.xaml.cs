using RobotController.Communication;
using RobotController.Communication.SerialStream;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        RobotConnection connection;
        public MainWindow()
        {
            InitializeComponent();
            LoadPortNames();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e) => CreateConnection();
        private void ComboboxPortsOnDropdownOpened(object sender, EventArgs e) => LoadPortNames();
        private void BtnDisconnect_Click(object sender, RoutedEventArgs e) => connection?.Dispose();

        private void CreateConnection()
        {
            var portName = PortsCombobox.Text;
            if (string.IsNullOrEmpty(portName)) return;

            var factory = new SerialPortFactory(portName);
            var port = factory.GetPort();
            var portManager = new SerialPortManager();
            portManager.TryOpen(port);
            
            var adapter = new SerialPortAdapter(port);
            connection = new RobotConnection(adapter);
        }

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
