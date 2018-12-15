using System;
using System.Windows;
using System.Windows.Controls;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for Navbar.xaml
    /// </summary>
    public partial class Navbar : UserControl
    {
        public Navbar()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> SerialPortDropDownOpened;
        private void OnSerialPortDropDownOpened(object sender, EventArgs e) =>
            SerialPortDropDownOpened?.Invoke(sender, e);

        public event EventHandler<RoutedEventArgs> ConnectButtonClicked;
        private void ConnectButtonClick(object sender, RoutedEventArgs e) =>
            ConnectButtonClicked?.Invoke(sender, e);

        public event EventHandler<RoutedEventArgs> DisconnectButtonClicked;
        private void DisconnectButtonClick(object sender, RoutedEventArgs e) =>
            DisconnectButtonClicked?.Invoke(sender, e);

        public event EventHandler<string> SelectedPortChanged;
        private void PortComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var port = (sender as ComboBox)?.SelectedItem as string;
            SelectedPortChanged?.Invoke(sender, port);
        }

        public event EventHandler<RoutedEventArgs> SendingButtonClicked;
        private void Button_OnSendingButtonClick(object sender, RoutedEventArgs e)
        {
            SendingButtonClicked?.Invoke(sender, e);
        }

        public event EventHandler<RoutedEventArgs> StartLoggingClicked;
        private void StartLoggingButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartLoggingClicked?.Invoke(sender, e);
        }

        public event EventHandler<RoutedEventArgs> StopLoggingClicked;
        private void StopLoggingButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopLoggingClicked?.Invoke(sender, e);
        }

        public event EventHandler<RoutedEventArgs> LoggingPathChanged;
        private void ButtonLogPath_OnClick(object sender, RoutedEventArgs e)
        {
            LoggingPathChanged?.Invoke(sender, e);
        }
    }
}
