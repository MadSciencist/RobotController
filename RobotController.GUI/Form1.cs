using RobotController.Communication;
using RobotController.Communication.SerialStream;
using System;
using System.Windows.Forms;

namespace RobotController.GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadPortNames();
        }



        private void LoadPortNames()
        {
            var ports = SerialPortUtils.GetAvailablePorts();

            if (ports.Length > 0)
            {
                ComboboxAvailablePorts.Items.Clear();
                ComboboxAvailablePorts.Items.AddRange(ports);
                ComboboxAvailablePorts.Text = ports[0];
            }
        }
        private void PortInUse(object o, ConnectionEventArgs args) => LabelConnectionStatus.Text = "Port already in use.";
        private void InvalidPortOperation(object o, ConnectionEventArgs args) => LabelConnectionStatus.Text = args.ExceptionMessage;
        private void OnConnect(object o, ConnectionEventArgs args) => LabelConnectionStatus.Text = "Connected";
        private void OnDisconnect(object o, ConnectionEventArgs args) => LabelConnectionStatus.Text = "Disconnected";


        private void ButtonRefresh_Click(object sender, EventArgs e) => LoadPortNames();
    }
}
