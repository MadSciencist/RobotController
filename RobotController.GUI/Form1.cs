using RobotController.Communication;
using System;
using System.Windows.Forms;

namespace RobotController.GUI
{
    public partial class Form1 : Form
    {
        private Connection _connection;
        public Form1()
        {
            InitializeComponent();

            _connection = new Connection();
            _connection.PortInUse += PortInUse;
            LoadPortNames();
        }



        private void LoadPortNames()
        {
            var ports = _connection.GetAvailablePorts();

            if (ports.Length > 0)
            {
                ComboboxAvailablePorts.Items.Clear();
                ComboboxAvailablePorts.Items.AddRange(ports);
                ComboboxAvailablePorts.Text = ports[0];
            }
        }
        private void PortInUse(object o, ConnectionEventArgs args) => LabelConnectionStatus.Text = "Port already in use.";

        private void ButtonRefresh_Click(object sender, EventArgs e) => LoadPortNames();

        private void ButtonConnect_Click(object sender, EventArgs e) => _connection.ConnectToPort(ComboboxAvailablePorts.Text);

        private void ButtonDisconnect_Click(object sender, EventArgs e) => _connection.Disconnect();
    }
}
