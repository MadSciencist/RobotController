using RobotController.RobotModels.PhysicalConverters;
using RobotController.WpfGui.ExtendedControls;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for RobotSettingsAlarms.xaml
    /// </summary>
    public partial class RobotSettingsAlarms : UserControl
    {
        public RobotSettingsAlarms()
        {
            InitializeComponent();
        }

        public event EventHandler<SendingTextBoxEventArgs> TextBoxEnterPressedNew;

        private void ExtendedTexBbox_OnEnterPressed(object sender, KeyEventArgs e)
        {
            var a = (ExtendedTexBbox)sender;
            var be = a.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();

            var sendingValue = VoltageConverter.GetBit(Convert.ToDouble(a.Text));
            Console.WriteLine(sendingValue);

            TextBoxEnterPressedNew?.Invoke(sender, new SendingTextBoxEventArgs {Value = sendingValue});
        }
    }

    public class SendingTextBoxEventArgs : EventArgs
    {

        public object Value { get; set; }
    }
}
