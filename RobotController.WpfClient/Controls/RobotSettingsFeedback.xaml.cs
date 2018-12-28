using RobotController.WpfGui.BusinessLogic;
using RobotController.WpfGui.ExtendedControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotController.WpfGui.Controls
{
    /// <summary>
    /// Interaction logic for RobotSettingsFeedback.xaml
    /// </summary>
    public partial class RobotSettingsFeedback : UserControl
    {
        public event EventHandler<SendingTextBoxEventArgs> TextBoxEnterPressed;
        public event EventHandler<RoutedEventArgs> CheckboxChecked;

        public RobotSettingsFeedback()
        {
            InitializeComponent();
        }

        private void ExtendedTexBbox_OnOnEnterPressed(object sender, KeyEventArgs e)
        {
            var textBox = (ExtendedTexBbox)sender;

            TextBoxEnterPressed?.Invoke(sender, new SendingTextBoxEventArgs {Value = textBox.Text});
        }

        private void CheckBox_OnChanged(object sender, RoutedEventArgs e)
        {
            CheckboxChecked?.Invoke(sender, e);
        }
    }
}
