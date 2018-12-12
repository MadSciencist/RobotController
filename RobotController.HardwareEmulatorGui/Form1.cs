using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace RobotController.HardwareEmulatorGui
{
    public partial class Form1 : Form
    {
		// pretty simple helper - hardware emulator
        private Tests.HardwareEmulator.HardwareEmulator _emulator;
        public Form1()
        {
            InitializeComponent();
            DropDown(this, EventArgs.Empty); //initialize ports
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            _emulator = new Tests.HardwareEmulator.HardwareEmulator(PortNamesComboBox.Text);
            _emulator.Received += (o, s) => Log(s);
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            _emulator.Dispose();
        }

        private void DropDown(object sender, EventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            PortNamesComboBox.Items.Clear();
            PortNamesComboBox.Items.AddRange(ports);
            PortNamesComboBox.Text = ports[0];
        }

        private void Log(string message)
        {
            var line = $"{DateTime.Now.ToString()} {message} {Environment.NewLine}";
            AppendToLogBox(line);
        }

        private void AppendToLogBox(string line)
        {
            this.Invoke((Action)delegate {
                LogBox.AppendText(line);
            });
        }
    }
}
