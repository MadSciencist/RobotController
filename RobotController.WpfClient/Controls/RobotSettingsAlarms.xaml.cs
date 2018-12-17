using RobotController.RobotModels.PhysicalConverters;
using RobotController.WpfGui.ExtendedControls;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using RobotController.Communication.Enums;
using RobotController.WpfGui.BusinessLogic;

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

        public event EventHandler<SendingTextBoxEventArgs> TextBoxEnterPressed;

        private void ExtendedTexBbox_OnEnterPressed(object sender, KeyEventArgs e)
        {
            var textBox = (ExtendedTexBbox)sender;

            //this is needed to force update RAW only view textboxes
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();

            object sendingValue;

            switch (textBox.ECommand)
            {
                case ESenderCommand.CriticalVoltageAlarm:
                case ESenderCommand.VoltageAlarm:
                    sendingValue = VoltageConverter.GetBit(Convert.ToDouble(textBox.Text));
                    break;

                case ESenderCommand.CurrentLeftAlarm:
                case ESenderCommand.CurrentRightAlarm:
                    sendingValue = CurrentConverter.GetBit(Convert.ToDouble(textBox.Text));
                    break;

                case ESenderCommand.CriticalTemperatureAlarm:
                case ESenderCommand.TemperatureAlarm:
                    sendingValue = TemperatureConverter.GetBit(Convert.ToDouble(textBox.Text));
                    break;

                default:
                    sendingValue = 0;
                    break;
            }

            TextBoxEnterPressed?.Invoke(sender, new SendingTextBoxEventArgs {Value = sendingValue.ToString()});
        }
    }
}
